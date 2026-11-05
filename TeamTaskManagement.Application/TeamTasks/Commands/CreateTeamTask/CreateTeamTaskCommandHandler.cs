using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.CreateTeamTask
{
    /// <summary>
    /// اجرا کننده دستور ایجاد وظیفه جدید در تیم
    /// </summary>
    internal sealed class CreateTeamTaskCommandHandler : ICommandHandler<CreateTeamTaskCommand, Guid> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly ITeamTaskRepository _teamTaskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateTeamTaskCommandHandler(
            ITeamRepository teamRepository,
            ITeamTaskRepository teamTaskRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {

            _teamRepository = teamRepository;
            _teamTaskRepository = teamTaskRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        #endregion

        /// <summary>
        /// ایجاد وظیفه جدید در تیم
        /// </summary>
        /// <returns>شناسه وظیفه ایجاد شده</returns>
        public async Task<Result<Guid>> Handle(CreateTeamTaskCommand request, CancellationToken ct) {

            // ایجاد عنوان و توضیحات وظیفه

            var titleResult = TaskTitle.Create(request.Title);
            if (titleResult.IsFailure)
                return titleResult.AsFailure<Guid>();

            var descriptionResult = TaskDescription.Create(request.Description);
            if (descriptionResult.IsFailure)
                return descriptionResult.AsFailure<Guid>();

            // دریافت اطلاعات تیم از دیتابیس
            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct);
            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult<Guid>();

            // دریافت اطلاعات عضویت کاربر فعلی در تیم
            var creatorMemberResult = team.GetActiveMember(_currentUserService.UserId);
            if (creatorMemberResult.IsFailure)
                return creatorMemberResult.AsFailure<Guid>();

            // دریافت اطلاعات سطح دسترسی عضو مسئول انجام وظیفه
            var assigneeMemberRoleResult = team
                .GetActiveMemberRole(request.AppointedToMemberId);

            if (assigneeMemberRoleResult.IsFailure)
                return assigneeMemberRoleResult.AsFailure<Guid>();

            // ایجاد پارامتر عضو محول کننده
            var creator = creatorMemberResult.Value.AsActor();

            // ایجاد پارامتر عضو مسئول انجام - کسی که کار بهش محول شده
            var assignee = new ActingMember(
                request.AppointedToMemberId,
                assigneeMemberRoleResult.Value);

            // ایجاد وظیفه در سطح اگریگیت
            var taskResult = TeamTask.Create(
                request.TeamId,
                creator,
                assignee,
                titleResult.Value,
                descriptionResult.Value,
                request.DeadlineUtc);

            if (taskResult.IsFailure)
                return taskResult.AsFailure<Guid>();

            // معرفی وظیفه جدید به ریپوزیتوری
            await _teamTaskRepository.InsertAsync(taskResult.Value, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(taskResult.Value.Id);
        }
    }
}