using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.ReadModels;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.Specifications.ForTeam;
using TeamTaskManagement.Persistence.Specifications;
using TeamTaskManagement.Persistence.Specifications.ForTeam;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Persistence.Repositories {
    /// <summary>
    /// پیاده کننده قرارداد دسترسی به اطلاعات اگریگیت تیم از دیتابیس
    /// </summary>
    public class TeamRepository : ITeamRepository {

        #region Context Injection

        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context) => _context = context;

        #endregion

        #region Any By Id

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cToken = default) {

            var domainSpec = new TeamByIdSpecification(id);

            return await _context.Set<Team>()
                .AnyAsync(domainSpec.ToExpression(), cToken);
        }

        #endregion

        #region Get By Id

        public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cToken = default) {

            var querySpec = new TeamByIdQuerySpecification(id);

            return await ApplyQuerySpecification(querySpec)
                .FirstOrDefaultAsync(cToken);
        }

        #endregion

        #region Get By Member Id

        public async Task<Team?> GetByMemberIdAsync(Guid memberId, CancellationToken cToken = default) {

            var querySpec = new TeamByMemberIdQuerySpecification(memberId);

            return await ApplyQuerySpecification(querySpec)
                .FirstOrDefaultAsync(cToken);
        }

        #endregion

        #region Group Get By User Id

        public async Task<IReadOnlyList<Team>> GetByUserIdAsync(Guid userId, bool onlyCurrentActiveTeams, CancellationToken cToken = default) {

            var querySpec = new TeamsByUserIdQuerySpecification(userId, onlyCurrentActiveTeams);

            return await ApplyQuerySpecification(querySpec)
                .ToListAsync(cToken);
        }

        #endregion

        #region Get Invites For User

        public async Task<IReadOnlyList<InviteReadModel>> GetInvitesForUserAsync
            (Guid userId,
             Guid? teamId,
             InviteDirectionFilter directionFilter,
             InviteStatusFilter statusFilter,
             CancellationToken ct = default) {

            // پایه کوئری
            var baseQuery =
                from invite in _context.Set<TeamInvite>()
                join team   in _context.Set<Team>()
                    on invite.HostTeamId equals team.Id
                join sender in _context.Set<TeamMember>()
                    on invite.SenderMemberId equals sender.Id
                select new {
                    Invite = invite,
                    Team = team,
                    Sender = sender
                };

            // فیلتر تیم، اختیاری
            if (teamId.HasValue) {
                baseQuery = baseQuery.Where(x => x.Invite.HostTeamId == teamId.Value);
            }

            // فیلتر جهت دعوتنامه
            baseQuery = directionFilter switch {

                InviteDirectionFilter.Sent =>
                    baseQuery.Where(x => x.Sender.UserId == userId),

                InviteDirectionFilter.Received =>
                    baseQuery.Where(x => x.Invite.ReceiverUserId == userId),

                _ => // All
                    baseQuery.Where(x =>
                        x.Sender.UserId == userId ||
                        x.Invite.ReceiverUserId == userId)
            };

            // فیلتر وضعیت
            baseQuery = statusFilter switch {

                InviteStatusFilter.Pending =>
                    baseQuery.Where(x => x.Invite.Status == TeamInviteStatus.Sent),

                InviteStatusFilter.NotPending =>
                    baseQuery.Where(x => x.Invite.Status != TeamInviteStatus.Sent),

                InviteStatusFilter.Accepted =>
                    baseQuery.Where(x => x.Invite.Status == TeamInviteStatus.Accepted),

                InviteStatusFilter.Rejected =>
                    baseQuery.Where(x => x.Invite.Status == TeamInviteStatus.Rejected),

                InviteStatusFilter.Cancelled =>
                    baseQuery.Where(x => x.Invite.Status == TeamInviteStatus.Cancelled),

                InviteStatusFilter.AutoCancelled =>
                    baseQuery.Where(x => x.Invite.Status == TeamInviteStatus.AutoCancelled),

                _ => // All
                    baseQuery
            };

            // Projection نهایی
            var result = await baseQuery
                .OrderByDescending(x => x.Invite.CreatedAtUtc)
                .Select(x => new InviteReadModel {

                    InviteId = x.Invite.Id,
                    TeamId = x.Team.Id,
                    TeamDisplayname = x.Team.TeamDisplayname.Value,

                    Status = x.Invite.Status,
                    SuggestedRoleTitle = x.Invite.SuggestedRoleTitle.Value,
                    SuggestedAccessLevel = x.Invite.SuggestedAccessLevel,

                    CreatedAtUtc = x.Invite.CreatedAtUtc,
                    LastModifiedAtUtc = x.Invite.LastModifiedAtUtc,

                    SenderMemberId = x.Sender.Id,
                    SenderUserId = x.Sender.UserId,
                    SenderRoleTitle = x.Sender.RoleDisplayname.Value,
                    SenderAccessLevel = x.Sender.AccessLevel,
                    SenderMembershipStatus = x.Sender.Status,
                    SenderJoinedAtUtc = x.Sender.CreatedAtUtc,
                    SenderLastStatusChangeAt = x.Sender.LastStatusChangeAt,

                    ReceiverUserId = x.Invite.ReceiverUserId
                })
                .ToListAsync(ct);

            return result.AsReadOnly();
        }

        #endregion

        #region Insert

        public async Task InsertAsync(Team team, CancellationToken cToken = default)
            => await _context.Set<Team>().AddAsync(team, cToken);

        public async Task InsertInviteAsync(TeamInvite invite, CancellationToken cToken = default)
            => await _context.Set<TeamInvite>().AddAsync(invite, cToken);

        public async Task InsertMemberAsync(TeamMember member, CancellationToken ct = default)
            => await _context.Set<TeamMember>().AddAsync(member, ct);

        #endregion

        #region Update

        public Task UpdateAsync(Team team, CancellationToken cToken = default) {

            // برای معرفی تغییرات تیم به کانتکست، از
            // منطق مشترک کلاس ریپوزیتوری-هلپر استفاده میکنیم
            var result = RepositoryHelper.UpdateEntityAsync(_context, team);

            // منطق داخل این ریجن خراب بود، کامنتش
            // کردم و در عوض برای ثبت و ویرایش
            // اعضا و دعوتنامه ها متد اینسرت و آپدیت
            // جدا به همین ریپوزیتوری اضافه کردم
            #region Obsolete Commented Code

            //// تک تک عضویت های تیم رو میگردیم
            //foreach (var member in team.MembersOfAllTimeHistory) {

            //    // ردیاب تغییر(چنج ترکر) هرکدوم رو میگیریم
            //    var entry = _context.Entry(member);

            //    // بررسی میکنیم تغییرات این عضویت ردیابی شده یا نه
            //    if (entry.State == EntityState.Detached)
            //        // اگر نشده بود، یعنی این عضویت جدیده
            //        // پس بعنوان اضافه شده علامتش میزنیم
            //        entry.State = EntityState.Added;
            //}

            //// همون کاررو برای دعوتنامه های تیم تکرار میکنیم
            //foreach (var invite in team.Invites) {
            //    var entry = _context.Entry(invite);
            //    if (entry.State == EntityState.Detached)
            //        entry.State = EntityState.Added;
            //}

            #endregion

            // نتیجه موفقیت آمیز برمیگردونیم
            return result;
        }

        public Task UpdateInviteAsync(TeamInvite invite, CancellationToken ct = default)
            => RepositoryHelper.UpdateEntityAsync(_context, invite);

        public Task UpdateMemberAsync(TeamMember member, CancellationToken ct = default)
            => RepositoryHelper.UpdateEntityAsync(_context, member);

        #endregion

        #region Internal-Shared Methods

        /// <summary>
        /// متد اعمال کننده کوئری-اسپاسیفیکیشن
        /// </summary>
        private IQueryable<Team> ApplyQuerySpecification(QuerySpecification<Team>
            querySpec) => SpecificationEvaluator.GetQuery(_context.Set<Team>(), querySpec);

        #endregion
    }
}
