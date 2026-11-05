using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Domain.Repositories {
    /// <summary>
    /// قرارداد دسترسی به اطلاعات اگریگیت وظیفه تیم از دیتابیس
    /// </summary>
    public interface ITeamTaskRepository {

        /// <summary>
        /// دریافت وظیفه بر اساس شناسه
        /// </summary>
        Task<TeamTask?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// دریافت تمام وظایف یک تیم
        /// </summary>
        Task<IReadOnlyList<TeamTask>> GetByTeamIdAsync(Guid teamId, CancellationToken ct = default);

        /// <summary>
        /// دریافت وظایف یک تیم با فیلتر وضعیت
        /// </summary>
        Task<IReadOnlyList<TeamTask>> GetByTeamIdAsync(
            Guid teamId,
            TeamTaskStatusFilter statusFilter = TeamTaskStatusFilter.All,
            CancellationToken ct = default);

        /// <summary>
        /// دریافت وظایف محول شده به یک عضو خاص
        /// </summary>
        Task<IReadOnlyList<TeamTask>> GetByAppointedMemberIdAsync(Guid memberId, CancellationToken ct = default);

        /// <summary>
        /// افزودن وظیفه جدید
        /// </summary>
        Task InsertAsync(TeamTask teamTask, CancellationToken ct = default);

        /// <summary>
        /// ویرایش وظیفه
        /// </summary>
        Task UpdateAsync(TeamTask teamTask, CancellationToken ct = default);

        /// <summary>
        /// بررسی وجود وظیفه
        /// </summary>
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// بررسی وجود وظیفه بسته نشده برای یک عضو تیم - چه بعنوان مسئول، چه بعنوان محول کننده
        /// </summary>
        Task<bool> HasOpenTasksForMemberAsync(Guid memberId, CancellationToken ct = default);

        /// <summary>
        /// دریافت گروهی تعداد وظایف باز مرتبط به اعضا
        /// </summary>
        /// <returns>
        /// در قالب یک دیکشنری، بیان میکند که هر عضو 
        /// چقدر وظیفه ناتمام بر اساس فیلتر ورودی دارد
        /// </returns>
        Task<IReadOnlyDictionary<Guid,
            (int AssignedCount,
             int CreatedCount,
             int SelfCount)>>
            GetOpenTasksCountsForMemberIdsAsync
            (IEnumerable<Guid> memberIds,
             CancellationToken ct = default);

        /// <summary>
        /// دریافت گروهی اطلاعات وظایف مربوط به لیستی 
        /// از اعضا، بهمراه فیلتر نوع نسبت وظیفه
        /// </summary>
        /// <returns>
        /// لیست کل وظایف مرتبط به
        /// اعضا بر اساس فیلتر ورودی
        /// </returns>
        Task<IReadOnlyCollection<TeamTask>> GetTasksForMemberIdsAsync
            (IEnumerable<Guid> memberIds,
             MemberTasksFilter filter,
             CancellationToken ct = default);

        Task<IReadOnlyList<TeamTask>> GetOpenTasksPastDeadlineAsync
            (int take, CancellationToken ct = default);
    }
}