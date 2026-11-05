using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Shared.Classes;
using static TeamTaskManagement.Domain.Errors.DomainErrors;

namespace TeamTaskManagement.Domain.Entities
{
    /// <summary>
    /// اگریگیت تیم
    /// </summary>
    public class Team : AggregateRoot {

        #region Internal Constructor & Lists

        /// <summary>
        /// لیست داخلی از کل اعضای فعلی و سابق تیم
        /// </summary>
        private readonly List<TeamMember> _members = new();

        /// <summary>
        /// لیست اعضای فعال - برای عملیات های دامین
        /// </summary>
        private IEnumerable<TeamMember> _activeMembers =>
            _members.Where(m => m.Status is TeamMemberStatus.Active);

        /// <summary>
        /// لیست دعوتنامه های ارسال شده توسط تیم
        /// </summary>
        private readonly List<TeamInvite> _invites = new();

        /// <summary>
        /// کانستراکتور داخلی
        /// </summary>
        private Team(Guid id, DisplayName teamDisplayName, TeamDescription description) : base(id) {

            TeamDisplayname = teamDisplayName;
            Description = description;
        }

        /// <summary>
        /// کانستراکتور برای ای-اف-کور تا بتونه مایگرشن بزنه
        /// </summary>
        #pragma warning disable CS8618
        private Team() : base(Guid.Empty) { }

        #endregion

        #region Domain Properties

        /// <summary>
        /// نام نمایشی تیم - تعیین و ویرایش توسط لیدر
        /// </summary>
        public DisplayName TeamDisplayname { get; private set; }

        /// <summary>
        /// توضیحات تیم - تعیین و ویرایش توسط لیدر
        /// </summary>
        public TeamDescription Description { get; private set; }

        /// <summary>
        /// لیست کل اعضای فعلی و سابق تیم
        /// </summary>
        public IReadOnlyCollection<TeamMember> MembersOfAllTimeHistory => _members;

        /// <summary>
        /// لیست اعضای فعلی تیم
        /// </summary>
        public IReadOnlyCollection<TeamMember> CurrentMembers =>
            _members.Where(m => m.Status is TeamMemberStatus.Active).ToList().AsReadOnly();

        /// <summary>
        /// لیست دعوتنامه های ارسال شده توسط تیم
        /// </summary>
        public IReadOnlyCollection<TeamInvite> Invites => _invites;

        #endregion

        #region Domain-Method: Create

        /// <summary>
        /// ایجاد تیم جدید - همزمان لیدر اولیه (سازنده تیم) نیز به عنوان عضو اضافه میشود
        /// </summary>
        public static Result<Team> Create(
            Guid leaderUserId,
            DisplayName teamDisplayName,
            TeamDescription teamDescription,
            DisplayName leaderRoleTitle) {

            var team = new Team(Guid.NewGuid(), teamDisplayName, teamDescription);

            var leaderResult = TeamMember.Create(leaderUserId, team.Id,
                TeamMemberStatus.Active, TeamMemberRole.Leader, leaderRoleTitle);

            if (leaderResult.IsFailure)
                return leaderResult.AsFailure<Team>();

            team._members.Add(leaderResult.Value);

            team.RaiseDomainEvent(new TeamCreatedEvent(team
                .Id, leaderUserId, teamDisplayName.Value));

            team.RaiseDomainEvent(new TeamMemberAddedEvent(team.Id,
                leaderResult.Value.Id, leaderUserId, TeamMemberRole.Leader));

            return team;
        }

        #endregion

        #region Domain-Method: Update

        /// <summary>
        /// ویرایش اطلاعات تیم - مجاز فقط توسط لیدر
        /// </summary>
        public Result<TeamMember> UpdateDetails
            (Guid actorMemberId,
             DisplayName displayName,
             TeamDescription description,
             DisplayName leaderRoleTitle) {

            var leaderCheck = EnsureIsLeader(actorMemberId);
            if (leaderCheck.IsFailure) return leaderCheck;

            TeamDisplayname = displayName;
            Description = description;
            LastModifiedAtUtc = DateTime.UtcNow;
            leaderCheck.Value.ChangeRoleTitle(leaderRoleTitle);

            RaiseDomainEvent(new TeamDetailsUpdatedEvent(Id,
                actorMemberId, displayName.Value, description.Value));

            return leaderCheck.Value;
        }

        #endregion

        #region Aggregate-Method: Send Invite

        /// <summary>
        /// ارسال دعوتنامه - فقط لیدر یا مدیر میانی مجازند
        /// </summary>
        public Result<TeamInvite> SendInvite(Guid senderMemberId,
            Guid receiverUserId, TeamMemberRole invitedRole, DisplayName invitedRoleTitle) {

            var sender = _activeMembers.FirstOrDefault(m => m.Id == senderMemberId);

            // فرستنده دعوتنامه باید عضو فعال تیم باشد
            if (sender is null)
                return DomainErrors.Team.SenderNotMember.AsResult<TeamInvite>();

            var senderIsMember = sender.AccessLevel is TeamMemberRole.Member;

            // عضو با سطح دسترسی معمولی مجاز به ارسال دعوتنامه نیست
            if (senderIsMember)
                return DomainErrors.Team.InsufficientPermission.AsResult<TeamInvite>();

            // عضو تیم نمیتواند به خودش دعوتنامه بفرستد
            if (sender.UserId == receiverUserId)
                return DomainErrors.Team.MemberCannotInviteItself.AsResult<TeamInvite>();

            // دعوتنامه نباید بتواند کاربر دعوت شونده را به مقام لیدر برساند
            if (invitedRole is TeamMemberRole.Leader)
                return DomainErrors.Team.TeamAlreadyHasLeader.AsResult<TeamInvite>();

            var senderIsNotLeader = sender.AccessLevel is not TeamMemberRole.Leader;

            var invitedRoleIsManager = invitedRole is TeamMemberRole.Manager;

            // مدیر میانی نمیتواند دعوتنامه با هدف جذب مدیر میانی بفرستد
            if (senderIsNotLeader && invitedRoleIsManager)
                return DomainErrors.Team.ManagerCannotInviteOthersInManagerRole.AsResult<TeamInvite>();

            // به کاربری که از قبل عضو تیم باشد نمیتوان دعوتنامه فرستاد
            if (_activeMembers.Any(m => m.UserId == receiverUserId))
                return DomainErrors.Team.MemberAlreadyExists.AsResult<TeamInvite>();

            // به کاربری که از قبل دعوتنامه ی باز از تیم داره نمیتوان دعوتنامه فرستاد
            if (_invites.Any(i => i.ReceiverUserId
                == receiverUserId && i.Status == TeamInviteStatus.Sent))
                return DomainErrors.Team.DuplicateInvite.AsResult<TeamInvite>();

            var inviteResult = TeamInvite.Create(Id,
                senderMemberId, receiverUserId, invitedRole, invitedRoleTitle);

            if (inviteResult.IsFailure) return inviteResult;

            _invites.Add(inviteResult.Value);

            RaiseDomainEvent(new TeamInviteSentEvent(
                Id,
                inviteResult.Value.Id,
                senderMemberId,
                sender.UserId,
                receiverUserId,
                TeamDisplayname.Value,
                invitedRoleTitle.Value));

            return inviteResult.Value;
        }

        #endregion

        #region Aggregate-Method: Accept Invite

        /// <summary>
        /// پذیرفتن دعوتنامه توسط کاربر دعوت شونده - عضو جدید اتمیک اضافه میشود
        /// </summary>
        public Result<TeamMember> AcceptInvite(Guid inviteId, Guid receiverUserId) {

            var invite = _invites.FirstOrDefault(i => i.Id == inviteId);
            if (invite is null) {
                return Result.Failure<TeamMember>(DomainErrors.Team.InviteNotFound);
            }

            // کاربر نباید بتونه دعوتنامه ی کاربر دعوت شونده دیگه ای رو پذیرش کنه 
            if (invite.ReceiverUserId != receiverUserId) {
                return Result.Failure<TeamMember>(DomainErrors.Team.InviteNotOwnedByUser);
            }

            // دعوتنامه ای که در وضعیت «فرستاده شده» نیست نمیتونه پذیرفته بشه
            if (invite.Status != TeamInviteStatus.Sent) {
                return Result.Failure<TeamMember>(DomainErrors.Team.InviteNotPending);
            }

            // اگر کاربر دعوت شونده از قبل عضو تیم باشد، دعوتنامه نامعتبر است و باید باطل شود
            if (_activeMembers.Any(m => m.UserId == receiverUserId)) {
                invite.AutoCancel();
                RaiseDomainEvent(new TeamInviteCancelledEvent(
                    Id,
                    invite.Id,
                    CancelledByMemberId: null,
                    IsAutoCancelled: true));
                return Result.Failure<TeamMember>(DomainErrors.Team.MemberAlreadyExists);
            }

            var memberResult = TeamMember.Create(receiverUserId, Id,
                TeamMemberStatus.Active, invite.SuggestedAccessLevel, invite.SuggestedRoleTitle);
            if (memberResult.IsFailure) {
                return memberResult;
            }

            _members.Add(memberResult.Value);
            invite.Accept();

            // اگر دعوتنامه های معلق دیگری برای همین کاربر در همین تیم وجود
            // داشته باشد (سناریوی ارسال همزمان چند دعوتنامه)، اکنون که کاربر
            // از طریق یکی از آنها عضو شده، بقیه باید بطور خودکار لغو شوند
            // تا در آینده(مثلا پس از حذف این عضو) دوباره قابل استفاده نباشند
            var staleInvites = _invites.Where(i =>
                i.Id != invite.Id &&
                i.ReceiverUserId == receiverUserId &&
                i.Status == TeamInviteStatus.Sent);

            foreach (var staleInvite in staleInvites) {
                staleInvite.AutoCancel();

                RaiseDomainEvent(new TeamInviteCancelledEvent(
                    Id,
                    staleInvite.Id,
                    CancelledByMemberId: null,
                    IsAutoCancelled: true));
            }

            LastModifiedAtUtc = DateTime.UtcNow;


            RaiseDomainEvent
                (new TeamInviteAcceptedEvent
                (Id,
                 invite.Id,
                 memberResult.Value.Id,
                 receiverUserId));

            RaiseDomainEvent
                (new TeamMemberAddedEvent
                (Id,
                 memberResult.Value.Id,
                 receiverUserId,
                 invite.SuggestedAccessLevel));

            return memberResult.Value;
        }

        #endregion

        #region Aggregate-Method: Reject Invite

        /// <summary>
        /// رد کردن دعوتنامه توسط کاربر دعوت شونده
        /// </summary>
        public Result<TeamInvite> RejectInvite(Guid inviteId, Guid receiverUserId) {

            var invite = _invites.FirstOrDefault(i => i.Id == inviteId);
            if (invite is null) return DomainErrors.Team.InviteNotFound.AsResult<TeamInvite>();

            // کاربر نباید بتونه دعوتنامه ی کاربر دعوت شونده دیگه ای رو رد کنه 
            if (invite.ReceiverUserId != receiverUserId)
                return DomainErrors.Team.InviteNotOwnedByUser.AsResult<TeamInvite>();

            // دعوتنامه ای که در وضعیت «فرستاده شده» نیست نمیتونه رد بشه
            if (invite.Status != TeamInviteStatus.Sent)
                return DomainErrors.Team.InviteNotPending.AsResult<TeamInvite>();

            invite.Reject();

            RaiseDomainEvent(new TeamInviteRejectedEvent(
                Id,
                invite.Id,
                receiverUserId));

            return invite;
        }

        #endregion

        #region Aggregate-Method: Cancel Invite

        /// <summary>
        /// لغو کردن دعوتنامه ی ارسال شده - مجاز توسط فرستنده یا لیدر
        /// </summary>
        public Result<TeamInvite> CancelInvite(Guid inviteId, Guid senderMemberId) {

            var sender = _activeMembers
                .FirstOrDefault(m => m.Id == senderMemberId);

            if (sender is null)
                return DomainErrors.Team.SenderNotMember.AsResult<TeamInvite>();

            var invite = _invites
                .FirstOrDefault(i => i.Id == inviteId);

            if (invite is null)
                return DomainErrors.Team.InviteNotFound.AsResult<TeamInvite>();

            // عضوی که لیدر نیست نباید بتونه دعوتنامه ای که خودش نفرستاده رو لغو کنه
            if (invite.SenderMemberId != senderMemberId
                && sender.AccessLevel != TeamMemberRole.Leader)
                return DomainErrors.Team.InviteNotSendedByMember.AsResult<TeamInvite>();
            
            // دعوتنامه ای که در وضعیت «فرستاده شده» نیست نمیتونه لغو بشه
            if (invite.Status != TeamInviteStatus.Sent)
                return DomainErrors.Team.InviteNotPending.AsResult<TeamInvite>();

            invite.Cancel();

            RaiseDomainEvent(new TeamInviteCancelledEvent(
                Id,
                invite.Id,
                senderMemberId,
                IsAutoCancelled: false));

            return invite;
        }

        #endregion

        #region Aggregate-Method: Remove Member

        /// <summary>
        /// حذف عضو توسط لیدر - غیرمجاز اگر عضو تنها لیدر باقیمانده تیم باشد
        /// </summary>
        public Result<TeamMember> RemoveMember(Guid actorMemberId, Guid memberId) {

            var leaderCheck = EnsureIsLeader(actorMemberId);

            if (leaderCheck.IsFailure) return leaderCheck.AsFailure<TeamMember>();

            var member = _activeMembers
                .FirstOrDefault(m => m.Id == memberId);

            if (member is null)
                return DomainErrors.Team.MemberNotFound.AsResult<TeamMember>();

            // در پیاده سازی فعلی تیم همیشه فقط یک لیدر دارد، پس این
            // چک عملا معادل «آیا عضو مورد نظر خود لیدر است» می باشد،
            // اما بصورت future-proof بر اساس شمارش نوشته شده است
            bool isLastLeader = member.AccessLevel == TeamMemberRole.Leader
                && _activeMembers.Count(m => m.AccessLevel == TeamMemberRole.Leader) == 1;

            if (isLastLeader)
                return DomainErrors.Team.CannotRemoveLeader.AsResult<TeamMember>();

            member.Remove();
            LastModifiedAtUtc = DateTime.UtcNow;

            RaiseDomainEvent(new TeamMemberRemovedEvent(
                Id,
                member.Id,
                member.UserId,
                actorMemberId));

            return member;
        }

        #endregion

        #region Aggregate-Method: Leave Team

        /// <summary>
        /// ترک تیم توسط خود عضو - لیدر نمیتواند تیم را ترک کند
        /// </summary>
        public Result<TeamMember> LeaveTeam(Guid memberIdToLeave) {

            var member = _activeMembers
                .FirstOrDefault(m => m.Id == memberIdToLeave);

            if (member is null)
                return DomainErrors.Team.MemberNotFound.AsResult<TeamMember>();

            // لیدر نمیتونه تیم رو ترک کنه
            if (member.AccessLevel == TeamMemberRole.Leader)
                return DomainErrors.Team.LeaderCannotLeaveTeam.AsResult<TeamMember>();

            member.Leave();
            LastModifiedAtUtc = DateTime.UtcNow;

            RaiseDomainEvent(new TeamMemberLeftEvent(
                Id,
                member.Id,
                member.UserId));

            return member;
        }

        #endregion

        #region Aggregate-Method: Change Member Role

        /// <summary>
        /// تغییر سطح دسترسی یک عضو - مجاز فقط توسط لیدر
        /// مقام لیدر از طریق این متد غیرقابل تنزل یا واگذاری است
        /// </summary>
        public Result<TeamMember> ChangeMemberRole(Guid actorMemberId, Guid memberId, TeamMemberRole newRole, DisplayName newRoleTitle) {

            // قاعده 1: فقط لیدر میتواند سطح دسترسی دیگران را تغییر دهد
            var leaderCheck = EnsureIsLeader(actorMemberId);
            if (leaderCheck.IsFailure) return leaderCheck.AsFailure<TeamMember>();

            var member = _activeMembers
                .FirstOrDefault(m => m.Id == memberId);

            if (member is null)
                return DomainErrors.Team.MemberNotFound.AsResult<TeamMember>();

            // قاعده 2: مقام لیدر از طریق این متد غیرقابل تنزل یا انتقال است
            // (واگذاری لیدری باید یک عملیات مجزا و آگاهانه باشد، نه یک تغییر نقش معمولی)
            if (member.AccessLevel == TeamMemberRole.Leader)
                return DomainErrors.Team.LeaderRoleIsImmutable.AsResult<TeamMember>();

            // قاعده 3: تیم همیشه و همیشه فقط یک لیدر میتواند داشته باشد، نه بیشتر
            // پس این متد هرگز نباید بتواند یک عضو دیگر را به لیدر ارتقا دهد
            if (newRole == TeamMemberRole.Leader)
                return DomainErrors.Team.TeamAlreadyHasLeader.AsResult<TeamMember>();

            member.ChangeRole(newRole, newRoleTitle);
            LastModifiedAtUtc = DateTime.UtcNow;

            return member;
        }

        #endregion

        // =======================================


        #region Helper-Method: Get User's Active Membership

        /// <summary>
        /// متد هلپر - پیدا کردن عضویت فعال کاربر بر اساس شناسه
        /// </summary>
        public Result<TeamMember> GetActiveMember(Guid userId) {

            var member = _activeMembers
                .FirstOrDefault(x => x.UserId == userId);

            if (member is null)
                return DomainErrors
                    .Team.MemberNotFound
                    .AsResult<TeamMember>();

            return Result.Success(member);
        }

        #endregion

        #region Helper-Method: Get Member's Role

        /// <summary>
        /// متد هلپر - دریافت سطح دسترسی عضو تیم
        /// </summary>
        public Result<TeamMemberRole> GetActiveMemberRole(Guid memberId) {

            var member = _activeMembers
                .FirstOrDefault(x => x.Id == memberId);

            if (member is null)
                return DomainErrors
                    .Team.MemberNotFound
                    .AsResult<TeamMemberRole>();

            return Result.Success(member.AccessLevel);
        }

        #endregion

        #region Internal-Shared Methods

        /// <summary>
        /// احراز اینکه عضو درخواست کننده، عضوی از این تیم و لیدر آن است
        /// </summary>
        private Result<TeamMember> EnsureIsLeader(Guid actorMemberId) {

            var actor = _activeMembers
                .FirstOrDefault(m => m.Id == actorMemberId);

            if (actor is null)
                return DomainErrors.Team.MemberNotFound.AsResult<TeamMember>();

            if (actor.AccessLevel != TeamMemberRole.Leader)
                return DomainErrors.Team.LeaderOnlyAction.AsResult<TeamMember>();

            return actor;
        }

        #endregion
    }
}
