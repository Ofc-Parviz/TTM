using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands
{
    /// <summary>
    /// کلاس هلپر دارای منطق مشترک 
    /// کامند های مربوط به وظیفه تیم
    /// </summary>
    internal sealed class TeamTaskCommandHelper {

        // اگر این کلاس نبود، کل منطق داخل این متد ها باید
        // توی همه کامند هندلر های وظیفه تیم تکرار میشد

        // بار مدیریت مراحل زیر از دوش
        // کامند هندلر ها برداشته شده است

        // 1 - دریافت اطلاعات وظیفه
        // 2 - دریافت اطلاعات تیم
        // 3 - دریافت اطلاعات عضویت کاربر فعلی
        // 4 - معرفی تغییرات به ریپوزیتوری بعد از عملیات در سطح دامین اگریگیت
        // 5 - اجرای عملیات دیتابیس و بازگشت نتیجه

        #region Injections

        private readonly ITeamTaskRepository _teamTaskRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public TeamTaskCommandHelper(
            ITeamTaskRepository teamTaskRepository,
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {

            _teamTaskRepository = teamTaskRepository;
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        #endregion

        #region Helper-Method: Team Task Operation

        /// <summary>
        /// کمک کننده عملیات وظیفه - اکسپرشن 
        /// حاوی عملیات اگریگیت را دریافت کرده 
        /// و موارد مشترک مانند : 
        /// بررسی صحت اطلاعات وظیفه، تیم، عضویت کاربر فعلی 
        /// و عملیات های دیتابیس بعد از عملیات اگریگیت
        /// </summary>
        public async Task<Result> TeamTaskOperationAsync(Guid teamTaskId,
            Func<TeamTask, ActingMember, Result> AggregateOperation, CancellationToken ct) {

            // بدون پارامتر دادن تیم، به منطق مرکزی داده میشود
            return await TeamTaskOperationAsync(teamTaskId,
                (team, task, actor) => AggregateOperation(task, actor), ct);
        }

        #endregion

        #region Helper-Method: Team Task Operation - Central Logic

        /// <summary>
        /// کمک کننده عملیات وظیفه - اکسپرشن 
        /// حاوی عملیات اگریگیت را دریافت کرده 
        /// و موارد مشترک مانند : 
        /// بررسی صحت اطلاعات وظیفه، تیم، عضویت کاربر فعلی 
        /// و عملیات های دیتابیس بعد از عملیات اگریگیت
        /// </summary>
        public async Task<Result> TeamTaskOperationAsync(Guid teamTaskId,
            Func<Team, TeamTask, ActingMember, Result> AggregateOperation, CancellationToken ct) {

            // دریافت اطلاعات وظیفه
            var task = await _teamTaskRepository.GetByIdAsync(teamTaskId, ct);
            if (task is null)
                return DomainErrors.TeamTask.TaskNotFound.AsResult();

            // دریافت اطلاعات تیم
            var team = await _teamRepository.GetByIdAsync(task.TeamId, ct);
            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult();

            // دریافت اطلاعات عضویت کاربر فعلی
            var currentMemberResult = team.GetActiveMember(_currentUserService.UserId);
            if (currentMemberResult.IsFailure)
                return currentMemberResult;

            // تبدیل شی عضو تیم به شی عضو درخواست کننده
            var actor = currentMemberResult.Value.AsActor();

            // انجام عملیات خواسته شده در سطح اگریگیت
            // این خط تنها منطق غیر مشترکه، که بعنوان پارامتر از هندلر دریافت میشه
            var result = AggregateOperation(team, task, actor);
            if (result.IsFailure)
                return result;

            // معرفی تغییرات وظیفه به ریپوزیتوری
            await _teamTaskRepository.UpdateAsync(task, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }

        #endregion
    }
}
