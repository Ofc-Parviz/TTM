using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.ReadModels;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Domain.Repositories {
    /// <summary>
    /// قرارداد دسترسی به اگریگیت تیم
    /// </summary>
    public interface ITeamRepository {

        /// <summary>
        /// دریافت تیم بر اساس شناسه (همراه با اعضا و دعوتنامه ها)
        /// </summary>
        Task<Team?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// دریافت تیم بر اساس شناسه عضو (برای پیدا کردن تیمی که عضو در آن است)
        /// </summary>
        Task<Team?> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default);

        /// <summary>
        /// دریافت تمام تیم هایی که یک کاربر در آن ها عضو است
        /// </summary>
        Task<IReadOnlyList<Team>> GetByUserIdAsync(Guid userId, bool onlyCurrentActiveTeams, CancellationToken ct = default);

        /// <summary>
        /// دریافت دعوتنامه های مرتبط با یک کاربر
        /// </summary>
        Task<IReadOnlyList<InviteReadModel>> GetInvitesForUserAsync
            (Guid userId,
             Guid? teamId,
             InviteDirectionFilter directionFilter,
             InviteStatusFilter statusFilter,
             CancellationToken ct = default);

        /// <summary>
        /// افزودن تیم جدید
        /// </summary>
        Task InsertAsync(Team team, CancellationToken ct = default);

        /// <summary>
        /// ویرایش تیم
        /// </summary>
        Task UpdateAsync(Team team, CancellationToken ct = default);

        /// <summary>
        /// بررسی وجود تیم
        /// </summary>
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// افزودن دعوتنامه جدید
        /// </summary>
        Task InsertInviteAsync(TeamInvite invite, CancellationToken ct = default);

        /// <summary>
        /// ویرایش دعوتنامه
        /// </summary>
        Task UpdateInviteAsync(TeamInvite invite, CancellationToken ct = default);

        /// <summary>
        /// افزودن عضو جدید
        /// </summary>
        Task InsertMemberAsync(TeamMember member, CancellationToken ct = default);

        /// <summary>
        /// ویرایش عضو
        /// </summary>
        Task UpdateMemberAsync(TeamMember member, CancellationToken ct = default);
    }
}