using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Application.Users.Queries;
using TeamTaskManagement.Application.Teams.Responses;
using TeamTaskManagement.Application.Users.Responses;
using static TeamTaskManagement.Shared.Classes.Constants;
using TeamTaskManagement.Domain.ReadModels;
using System.ComponentModel;
using TeamTaskManagement.Domain.Shared;

namespace TeamTaskManagement.Application.Teams.Queries
{
    /// <summary>
    /// مپ کردن ریسپانس های مربوط به تیم
    /// </summary>
    internal static class TeamMappings {

        #region Map Team Response - Simple

        /// <summary>
        /// تبدیل اطلاعات خام تیم به ریسپانس مناسب
        /// </summary>
        public static TeamResponse MapTeamResponse
            (Team? team,
             MemberResponse currentUserMembership,
             IReadOnlyList<MemberResponse> members,
             IReadOnlyList<InviteResponse> pendingInvites) {

            return new TeamResponse
                (team?.Id ?? Guid.Empty,
                 team?.TeamDisplayname.Value ?? NotAvailable,
                 team?.Description.Value ?? NotAvailable,
                 team?.CreatedAtUtc ?? DateTime.MinValue,
                 team?.LastModifiedAtUtc ?? DateTime.MinValue,
                 currentUserMembership,
                 members,
                 pendingInvites);
        }

        #endregion

        #region Map Team Response - Using Dictionary

        /// <summary>
        /// تبدیل اطلاعات خام تیم به ریسپانس
        /// مناسب - دریافت ایمیل ها و نام های نمایشی از دیکشنری
        /// </summary>
        public static TeamResponse MapTeamResponse
            (Guid currentUserId,
             Team? team,
             TeamMember? currentUserMembership,
             bool includeInvites,
             IReadOnlyDictionary<Guid, (string Email, string Displayname)> dict,
             bool onlyPendingInvites = true) {

            var currentUserMembershipResp = MapMemberResponse(currentUserMembership, dict);

            var members = MapMemberCollectionResponse(team?
                .CurrentMembers ?? Array.Empty<TeamMember>(), dict);

            var invites = includeInvites
                ? MapInviteCollectionResponse(currentUserId, team, onlyPendingInvites, dict)
                : Array.Empty<InviteResponse>();

            return MapTeamResponse(team, currentUserMembershipResp, members, invites);
        }

        #endregion

        #region Map Member Response - Simple

        /// <summary>
        /// تبدیل اطلاعات خام عضویت تیم به ریسپانس مناسب
        /// </summary>
        public static MemberResponse MapMemberResponse
            (TeamMember? member, string userEmail, string userDisplayname) {

            return new MemberResponse
                (member?.Id ?? Guid.Empty,
                 member?.UserId ?? Guid.Empty,
                 userEmail,
                 userDisplayname,
                 member?.RoleDisplayname.Value ?? NotAvailable,
                 member?.AccessLevel.MemberAccessLevelRoleToString() ?? NotAvailable,
                 member?.AccessLevel.MapMemberRoleResponse() ?? MemberRoleResponse.Member,
                 member?.Status.MembershipStatusToString() ?? NotAvailable,
                 member?.CreatedAtUtc ?? DateTime.MinValue,
                 member?.LastStatusChangeAt ?? DateTime.MinValue);
        }

        #endregion

        #region Map Member Response - Using Dictionary

        /// <summary>
        /// تبدیل اطلاعات خام عضویت تیم به ریسپانس
        /// مناسب - دریافت ایمیل و نام نمایشی از دیکشنری
        /// </summary>
        public static MemberResponse MapMemberResponse
            (TeamMember? member,
             IReadOnlyDictionary<Guid,
                 (string Email, string Displayname)> dict) {

            var (email, displayname) = UserMappings
                .MapDictionaryItem(member?.UserId ?? Guid.Empty, dict);

            return MapMemberResponse(member, email, displayname);
        }

        /// <summary>
        /// تبدیل اطلاعات خام مدل خواندنی 
        /// دعوتنامه به ریسپانس مناسب عضویت فرستنده
        /// دریافت ایمیل و نام نمایشی از دیکشنری
        /// </summary>
        public static MemberResponse MapMemberResponse
            (InviteReadModel? inviteModel,
             IReadOnlyDictionary<Guid,
                 (string Email, string Displayname)> dict) {

            var (email, displayname) = UserMappings
                .MapDictionaryItem(inviteModel?.SenderUserId ?? Guid.Empty, dict);

            return new MemberResponse(
                    inviteModel?.SenderMemberId ?? Guid.Empty,
                    inviteModel?.SenderUserId ?? Guid.Empty,
                    email,
                    displayname,
                    inviteModel?.SenderRoleTitle ?? NotAvailable,
                    inviteModel?.SenderAccessLevel.MemberAccessLevelRoleToString() ?? NotAvailable,
                     inviteModel?.SenderAccessLevel.MapMemberRoleResponse() ?? MemberRoleResponse.Member,
                    inviteModel?.SenderMembershipStatus.MembershipStatusToString() ?? NotAvailable,
                    inviteModel?.SenderJoinedAtUtc ?? DateTime.MinValue,
                    inviteModel?.SenderLastStatusChangeAt ?? DateTime.MinValue);
        }

        #endregion

        #region Map Member Collection Response

        /// <summary>
        /// تبدیل اطلاعات خام لیست عضویت های تیم به لیستی از ریسپانس
        /// های مناسب - دریافت ایمیل ها و نام های نمایشی از دیکشنری
        /// </summary>
        public static IReadOnlyList<MemberResponse> MapMemberCollectionResponse
            (IEnumerable<TeamMember> members,
             IReadOnlyDictionary<Guid, (string Email, string Displayname)> dict)
            => members.Select(member => MapMemberResponse(member, dict)).ToList();

        #endregion

        #region Map Member Role Response

        public static MemberRoleResponse MapMemberRoleResponse(this TeamMemberRole role) {

            switch (role) {

                case TeamMemberRole.Leader: return MemberRoleResponse.Leader;

                case TeamMemberRole.Manager: return MemberRoleResponse.Manager;

                case TeamMemberRole.Member: return MemberRoleResponse.Member;

                default: return MemberRoleResponse.Member;
            }
        }

        #endregion

        #region Map Invite Response - Simple

        /// <summary>
        /// تبدیل اطلاعات خام دعوتنامه تیم به ریسپانس مناسب
        /// </summary>
        public static InviteResponse MapInviteResponse
            (TeamInvite? invite,
             Team? team,
             InviteDirectionResponse inviteDirection,
             MemberResponse sender,
             UserSimpleResponse receiver) {

            return new InviteResponse
                (invite?.Id ?? Guid.Empty,
                 invite?.HostTeamId ?? Guid.Empty,
                 team?.TeamDisplayname.Value ?? NotAvailable,
                 inviteDirection,
                 invite?.Status ?? (TeamInviteStatus)(-1),
                 invite?.Status.InviteStatusToString() ?? NotAvailable,
                 invite?.SuggestedRoleTitle.Value ?? NotAvailable,
                 invite?.SuggestedAccessLevel.MemberAccessLevelRoleToString() ?? NotAvailable,
                 sender,
                 receiver,
                 invite?.CreatedAtUtc ?? DateTime.MinValue,
                 invite?.LastModifiedAtUtc ?? DateTime.MinValue);
        }

        #endregion

        #region Map Invite Response - Using Dictionary

        /// <summary>
        /// تبدیل اطلاعات خام دعوتنامه تیم به ریسپانس
        /// مناسب - دریافت ایمیل و نام نمایشی از دیکشنری
        /// </summary>
        public static InviteResponse MapInviteResponse
           (Guid currentUserId,
            TeamInvite? invite,
            Team? team,
            TeamMember? sender,
            IReadOnlyDictionary<Guid, (string Email, string Displayname)> dict) {

            var direction = MapInviteDirectionResponse(currentUserId, invite, sender);

            var senderResp = MapMemberResponse(sender, dict);

            var receiverResp = UserMappings
                .MapUserSimpleResponse(invite?
                .ReceiverUserId ?? Guid.Empty, dict);

            return MapInviteResponse(invite, team, direction, senderResp, receiverResp);
        }

        #endregion

        #region Map Invite Response - Using Dictionary & Read Model

        public static InviteResponse MapInviteResponse
            (Guid currentUserId, InviteReadModel inviteModel,
             IReadOnlyDictionary<Guid, (string Email, string Displayname)> dict) {

            return new InviteResponse
                (inviteModel.InviteId,
                 inviteModel.TeamId,
                 inviteModel.TeamDisplayname,
                 MapInviteDirectionResponse(currentUserId, inviteModel),
                 inviteModel.Status,
                 inviteModel.Status.InviteStatusToString(),
                 inviteModel.SuggestedRoleTitle,
                 inviteModel.SuggestedAccessLevel.MemberAccessLevelRoleToString(),
                 MapMemberResponse(inviteModel, dict),
                 UserMappings.MapUserSimpleResponse(inviteModel.ReceiverUserId, dict),
                 inviteModel.CreatedAtUtc,
                 inviteModel.LastModifiedAtUtc);
        }

        #endregion

        #region Map Invite Collection Response

        /// <summary>
        /// تبدیل اطلاعات خام دعوتنامه های تیم به لیستی از ریسپانس
        /// های مناسب - دریافت ایمیل ها و نام های نمایشی از دیکشنری
        /// </summary>
        public static IReadOnlyList<InviteResponse> MapInviteCollectionResponse
            (Guid currentUserId, Team? team, bool onlyPendings,
             IReadOnlyDictionary<Guid, (string Email, string Displayname)> dict) {

            if(team is null) return Array.Empty<InviteResponse>();

            var invites = team.Invites
                .Where(invite => onlyPendings ? invite
                .Status is TeamInviteStatus.Sent : true);

            return invites.Select
                (invite => MapInviteResponse(currentUserId, invite, team,
                 team.MembersOfAllTimeHistory
                     .FirstOrDefault(member => member
                     .Id == invite.SenderMemberId), dict)).ToList();
        }

        #endregion

        #region Map Invite Direction Response

        /// <summary>
        /// مپ کردن جهت دعوتنامه نسبت به کاربر فعلی
        /// </summary>
        public static InviteDirectionResponse MapInviteDirectionResponse
            (Guid currentUserId,
             TeamInvite? invite,
             TeamMember? sender) {

            // اگر اطلاعات ورودی ناقص بود
            if (invite is null ||
                sender is null ||
                currentUserId == Guid.Empty)
                // میگوییم جهت دعوتنامه نامشخص است
                return InviteDirectionResponse.Unknown;

            // اگر فرستنده و گیرنده یک نفر باشد
            if (invite.ReceiverUserId == sender.UserId
                // یا اگر فرستنده مندرج در دعوتنامه
                // با فرستنده ورودی متناقض باشد
                || invite.SenderMemberId != sender.Id)
                // میگوییم دعوتنامه نامعتبر است
                return InviteDirectionResponse.Invalid;

            // اگر شناسه گیرنده همان شناسه کاربر فعلی باشد
            if (invite.ReceiverUserId == currentUserId)
                // میگوییم کاربر فعلی گیرنده این دعوتنامه ست
                return InviteDirectionResponse.ReceivedByUser;

            // اگر شناسه فرستنده همان شناسه کاربر فعلی باشد
            if (sender.UserId == currentUserId)
                // میگوییم کاربر فعلی فرستنده این دعوتنامه ست
                return InviteDirectionResponse.SentByUser;

            // اگر شروط قبلی صدق نکرد، کاربر فعلی
            // قطعا هیچ نقشی در این دعوتنامه ندارد
            return InviteDirectionResponse.None;
        }

        #endregion

        #region Map Invite Direction Response - Using Invite Read Model

        /// <summary>
        /// مپ کردن جهت دعوتنامه نسبت به کاربر فعلی
        /// </summary>
        public static InviteDirectionResponse MapInviteDirectionResponse
            (Guid currentUserId,
             InviteReadModel? inviteModel) {

            // اگر اطلاعات ورودی ناقص بود
            if (inviteModel is null ||
                currentUserId == Guid.Empty)
                // میگوییم جهت دعوتنامه نامشخص است
                return InviteDirectionResponse.Unknown;

            // اگر فرستنده و گیرنده یک نفر باشد
            if (inviteModel.ReceiverUserId == inviteModel.SenderUserId)
                // میگوییم دعوتنامه نامعتبر است
                return InviteDirectionResponse.Invalid;

            // اگر شناسه گیرنده همان شناسه کاربر فعلی باشد
            if (inviteModel.ReceiverUserId == currentUserId)
                // میگوییم کاربر فعلی گیرنده این دعوتنامه ست
                return InviteDirectionResponse.ReceivedByUser;

            // اگر شناسه فرستنده همان شناسه کاربر فعلی باشد
            if (inviteModel.SenderUserId == currentUserId)
                // میگوییم کاربر فعلی فرستنده این دعوتنامه ست
                return InviteDirectionResponse.SentByUser;

            // اگر شروط قبلی صدق نکرد، کاربر فعلی
            // قطعا هیچ نقشی در این دعوتنامه ندارد
            return InviteDirectionResponse.None;
        }

        #endregion
    }
}
