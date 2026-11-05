using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.Specifications.ForTeamTask;
using TeamTaskManagement.Persistence.Specifications;
using TeamTaskManagement.Persistence.Specifications.ForTeamTask;
using TeamTaskManagement.Domain.Shared;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Persistence.Repositories {
    /// <summary>
    /// پیاده کننده قرارداد دسترسی به اطلاعات اگریگیت وظیفه تیم از دیتابیس
    /// </summary>
    public class TeamTaskRepository : ITeamTaskRepository {

        #region Context Injection

        private readonly ApplicationDbContext _context;

        public TeamTaskRepository(ApplicationDbContext context) => _context = context;

        #endregion

        #region Any By Id

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) {

            var spec = new TeamTaskByIdSpecification(id);

            return await _context.Set<TeamTask>()
                .AnyAsync(spec.ToExpression(), ct);
        }

        #endregion

        #region Any Open By Member Id

        public async Task<bool> HasOpenTasksForMemberAsync
            (Guid memberId, CancellationToken ct = default) {

            // برای چک ساده ی وجود وظیفه، مستقیم از
            // دامین-اسپاسیفیکیشن مرتبط استفاده میکنیم
            var spec = new OpenTasksForMemberSpecification(memberId);

            return await _context.Set<TeamTask>()
                .AnyAsync(spec.ToExpression(), ct);
        }

        #endregion

        #region Get By Id

        public async Task<TeamTask?> GetByIdAsync
            (Guid id, CancellationToken ct = default) {

            var querySpec = new TeamTaskByIdQuerySpecification(id);

            return await ApplyQuerySpecification(querySpec)
                .FirstOrDefaultAsync(ct);
        }

        #endregion

        #region Get By Team Id

        public async Task<IReadOnlyList<TeamTask>> GetByTeamIdAsync
            (Guid teamId, CancellationToken ct = default) {

            var querySpec = new TeamTasksByTeamIdQuerySpecification(teamId);

            return await ApplyQuerySpecification(querySpec)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<TeamTask>> GetByTeamIdAsync
            (Guid teamId,
             TeamTaskStatusFilter statusFilter = TeamTaskStatusFilter.All,
             CancellationToken ct = default) {

            var query = _context.Set<TeamTask>()
                .Where(t => t.TeamId == teamId);

            /*
            چون انتیتی فریمورک نمیتونه اکستنشن متد هایی مثل
            ItsOpenTaskStatus()
            رو به اس-کیو-ال ترجمه کنه، شرط ها رو صریح نوشتم
            این با قاعده دامین سازگاره چون همون منطق مرکزی ملاکه
             */

            query = statusFilter switch {
                TeamTaskStatusFilter.Open =>
                    query.Where(t =>
                        t.Status != TeamTaskStatus.ClosedAsComplete &&
                        t.Status != TeamTaskStatus.ClosedAsFailure &&
                        t.Status != TeamTaskStatus.ClosedAsDeadlineFailure),

                TeamTaskStatusFilter.Closed =>
                    query.Where(t =>
                        t.Status == TeamTaskStatus.ClosedAsComplete ||
                        t.Status == TeamTaskStatus.ClosedAsFailure ||
                        t.Status == TeamTaskStatus.ClosedAsDeadlineFailure),

                _ => query
            };

            return await query
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync(ct);
        }

        #endregion

        #region Get By Appointed Member Id

        public async Task<IReadOnlyList<TeamTask>> GetByAppointedMemberIdAsync
            (Guid memberId, CancellationToken ct = default) {

            var querySpec = new TeamTasksByAppointedMemberIdQuerySpecification(memberId);

            return await ApplyQuerySpecification(querySpec)
                .ToListAsync(ct);
        }

        #endregion

        #region Group Get For a List of Members

        public async Task <IReadOnlyCollection<TeamTask>> GetTasksForMemberIdsAsync
            (IEnumerable<Guid> memberIds,
             MemberTasksFilter filter,
             CancellationToken ct = default) {

            // تبدیل شناسه های دریافت شده به هش ست یکتا
            var ids = memberIds?.ToHashSet() ?? new HashSet<Guid>();

            // اگر لیست شناسه ها خالی بود، لیست خالی برمیگرده
            if (ids.Count is 0)
                return Array.Empty<TeamTask>().AsReadOnly();

            // کوئری دریافت وظایف مرتبط با عضویت های ورودی بر اساس فیلتر
            var expectedQuery = GetExpectedQueryByFilter(filter, ids);

            // کوئری یکباره برای جلوگیری از مشکل N + 1
            var results = await _context.Set<TeamTask>()
                // اعمال کوئری و متریالایز کردن
                .Where(expectedQuery).ToListAsync(ct);
            // یادم باشه بعدا برای این کوئری اسپاسیفیکیشن بنویسم
            // # Check Later

            return results.AsReadOnly();
        }

        #endregion

        #region Group Get Open-Task Counts For Members

        public async Task
            <IReadOnlyDictionary<Guid,
            (int AssignedCount,
             int CreatedCount,
             int SelfCount)>>
            GetOpenTasksCountsForMemberIdsAsync
            (IEnumerable<Guid> memberIds, CancellationToken ct = default) {

            // تبدیل شناسه های دریافت شده به هش ست یکتا
            var ids = memberIds?.ToHashSet() ?? new HashSet<Guid>();

            // اگر لیست شناسه ها خالی بود، دیکشنری خالی برمیگرده
            if (ids.Count is 0)
                return new Dictionary<Guid, (int, int, int)>().AsReadOnly();

            // همه وظایف باز مرتبط با این اعضا را یکجا می‌گیریم
            var openTasks = await _context.Set<TeamTask>()
                .Where(t => DomainConstants.TeamTaskOpenStatuses.Contains(t.Status)
                 && (ids.Contains(t.AppointedToMemberId)
                 || ids.Contains(t.CreatedByMemberId)))
                .Select(t => new {
                    t.AppointedToMemberId,
                    t.CreatedByMemberId
                })
                .ToListAsync(ct);

            var result = new Dictionary<Guid,
                (int AssignedCount, int CreatedCount, int SelfCount)>();

            foreach (var memberId in ids) {

                int assignedCount = openTasks.Count(t =>
                    t.AppointedToMemberId == memberId &&
                    t.CreatedByMemberId != memberId);

                int createdCount = openTasks.Count(t =>
                    t.CreatedByMemberId == memberId &&
                    t.AppointedToMemberId != memberId);

                int selfCount = openTasks.Count(t =>
                    t.AppointedToMemberId == memberId &&
                    t.CreatedByMemberId == memberId);

                result[memberId] = (assignedCount, createdCount, selfCount);
            }

            return result.AsReadOnly();
        }

        public async Task<IReadOnlyList<TeamTask>> GetOpenTasksPastDeadlineAsync
            (int take, CancellationToken ct = default) {

            var utcNow = DateTime.UtcNow;

            return await _context.Set<TeamTask>()
                .Where(t => t.DeadlineUtc != null
                         && t.DeadlineUtc < utcNow
                         && DomainConstants.TeamTaskOpenStatuses.Contains(t.Status))
                .OrderBy(t => t.DeadlineUtc)
                .Take(take)
                .ToListAsync(ct);
        }

        #endregion

        #region Insert

        public async Task InsertAsync
            (TeamTask teamTask, CancellationToken ct = default)
            => await _context.Set<TeamTask>().AddAsync(teamTask, ct);

        #endregion

        #region Update

        public Task UpdateAsync
            (TeamTask teamTask, CancellationToken ct = default)
            // برای معرفی تغییرات وظیفه به کانتکست، از
            // منطق مشترک کلاس ریپوزیتوری-هلپر استفاده میکنیم
            => RepositoryHelper.UpdateEntityAsync(_context, teamTask);

        #endregion

        #region Obsoleted Repository Implimentations

        // این ریجن مال پیاده سازی های قبلی این ریپوزیتوریه، به
        // محض تکمیل پیاده یازی جدید(تعداد وظایف)، این ریجن رو حذف میکنم

        public async Task<IReadOnlyDictionary<Guid, int>>
            GetOpenTasksCountsForAssigneeMemberIdsAsync
            (IEnumerable<Guid> appointedToMemberIds, CancellationToken ct = default) {

            // تبدیل شناسه های دریافت شده به هش ست یکتا
            var ids = appointedToMemberIds?.ToHashSet() ?? new HashSet<Guid>();

            // اگر لیست شناسه ها خالی بود، دیکشنری خالی برمیگرده
            if (ids.Count is 0)
                return new Dictionary<Guid, int>().AsReadOnly();

            var openStatuses = new HashSet<TeamTaskStatus> {
                TeamTaskStatus.Pending,
                TeamTaskStatus.OnGoing,
                TeamTaskStatus.ClaimedAsComplete,
                TeamTaskStatus.ClaimedAsFailure,
                TeamTaskStatus.OrderedToTryAgain,
                TeamTaskStatus.AcceptedToTryAgain,
                TeamTaskStatus.RejectedToTryAgain
            };

            // کوئری یکباره برای جلوگیری از مشکل N + 1
            var results = await _context.Set<TeamTask>()
                // از وظایفی که بازن و شناسه انجام دهنده شون توی لیست ورودیه
                .Where(task =>
                    ids.Contains(task.AppointedToMemberId)
                    && openStatuses.Contains(task.Status))
                // گروه بندی بر اساس شناسه انجام دهنده
                .GroupBy(task => task.AppointedToMemberId)
                // دریافت تعداد وظایف در هر گروه بندی
                .Select(group => new {
                    MemberId = group.Key,
                    Count = group.Count()
                })
                // اجرای کوئری و متریالایز کردن
                .ToListAsync(ct);
            // یادم باشه بعدا برای این کوئری اسپاسیفیکیشن بنویسم
            // # Check Later

            // تبدیل نتایج به دیکشنری فقط خوندنی
            return results.ToDictionary
                (key => key.MemberId, value => value.Count).AsReadOnly();
        }

        public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<TeamTask>>>
            GetTasksForAssigneeMemberIdsAsync
            (IEnumerable<Guid> appointedToMemberIds, CancellationToken ct = default) {

            // تبدیل شناسه های دریافت شده به هش ست یکتا
            var ids = appointedToMemberIds?.ToHashSet() ?? new HashSet<Guid>();

            // اگر لیست شناسه ها خالی بود، دیکشنری خالی برمیگرده
            if (ids.Count is 0)
                return new Dictionary<Guid, IReadOnlyCollection<TeamTask>>().AsReadOnly();

            // کوئری یکباره برای جلوگیری از مشکل N + 1
            var results = await _context.Set<TeamTask>()
                // درخواست وظایف محول شده مرتبط با عضویت های ورودی
                .Where(task => ids.Contains(task.AppointedToMemberId))
                // اجرای کوئری و متریالایز کردن
                .ToListAsync(ct);
            // یادم باشه بعدا برای این کوئری اسپاسیفیکیشن بنویسم
            // # Check Later

            // تبدیل نتایج به دیکشنری فقط خوندنی
            return ids
                .ToDictionary(key => key, value => results
                .Where(t => t.AppointedToMemberId == value)
                .ToList() as IReadOnlyCollection<TeamTask>)
                .AsReadOnly();
        }

        #endregion

        #region Internal-Shared Methods

        /// <summary>
        /// متد اعمال کننده کوئری-اسپاسیفیکیشن
        /// </summary>
        private IQueryable<TeamTask> ApplyQuerySpecification(QuerySpecification<TeamTask>
            querySpec) => SpecificationEvaluator.GetQuery(_context.Set<TeamTask>(), querySpec);


        /// <summary>
        /// دریافت کوئری بر اساس فیلتر
        /// </summary>
        /// <exception cref="InvalidOperationException">در صورت اینام نامعتبر</exception>
        private Expression<Func<TeamTask, bool>> GetExpectedQueryByFilter
            (MemberTasksFilter filter, HashSet<Guid> memberIds) {

            switch (filter) {

                // وظایفی که اعضا فقط مدیریتشون میکنن
                // و انجامشون به عهده کس دیگه ایه
                case MemberTasksFilter.CreatedTasks:
                    return task => task
                    .AppointedToMemberId != task
                    .CreatedByMemberId && memberIds
                    .Contains(task.CreatedByMemberId);

                // وظایفی که اعضا فقط انجامشون میدن
                // و مدیریت اونها به عهده کس دیگه ایه
                case MemberTasksFilter.AssignedTasks:
                    return task => task
                    .AppointedToMemberId != task
                    .CreatedByMemberId && memberIds
                    .Contains(task.AppointedToMemberId);

                // وظایفی که اعضا هم مدیر هم انجام دهندشونن
                case MemberTasksFilter.SelfTasks:
                    return task => task
                        .AppointedToMemberId == task
                        .CreatedByMemberId && memberIds
                        .Contains(task.AppointedToMemberId);

                // همه وظایف مربوط به اعضا
                default: // MemberTasksFilter.AllTasks:
                    return task
                        => memberIds.Contains(task.CreatedByMemberId)
                        || memberIds.Contains(task.AppointedToMemberId);
            }
        }

        #endregion
    }
}
