using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.CreateTeam {
    /// <summary>
    /// اجرا کننده دستور ایجاد تیم جدید
    /// </summary>
    internal sealed class CreateTeamCommandHandler : ICommandHandler<CreateTeamCommand, Guid> {

        #region Repository Injection

        private readonly IUnitOfWork _unit;
        private readonly ITeamRepository _teamRepository;
        private readonly ICurrentUserService _currentUserService;
        public CreateTeamCommandHandler(
            ICurrentUserService currentUserServices,
            ITeamRepository teamRepository,
            IUnitOfWork unit) {

            _teamRepository = teamRepository;
            _currentUserService = currentUserServices;
            _unit = unit;
        }

        #endregion

        /// <summary>
        /// عملیات ایجاد تیم جدید
        /// </summary>
        /// <returns>شناسه تیم ایجاد شده</returns>
        public async Task<Result<Guid>> Handle(CreateTeamCommand request, CancellationToken cToken) {

            // گرفتن شناسه کاربر فعلی
            var currentUserId = _currentUserService.UserId;


            // اعتبارسنجی نام نمایشی تیم
            var teamDNameResult = DisplayName.Create(request.TeamDisplayname);

            if (teamDNameResult.IsFailure) return teamDNameResult.AsFailure<Guid>();

            // اعتبارسنجی توضیحات تیم
            var teamDescResult = TeamDescription.Create(request.TeamDescription);

            if (teamDescResult.IsFailure) return teamDescResult.AsFailure<Guid>();

            // اعتبارسنجی نام نمایشی لیدر
            var leaderDNameResult = DisplayName.Create(request.LeaderTitle);

            if (leaderDNameResult.IsFailure) return leaderDNameResult.AsFailure<Guid>();


            // تلاش برای ایجاد شی تیم
            var newTeamResult = Team.Create(currentUserId,
                teamDNameResult.Value, teamDescResult.Value, leaderDNameResult.Value);

            if (newTeamResult.IsFailure) return newTeamResult.AsFailure<Guid>();


            // معرفی شی تیم ایجاد شده به ریپوزیتوری
            await _teamRepository.InsertAsync(newTeamResult.Value, cToken);

            // اجرای عملیات دیتابیس
            await _unit.SaveChangesAsync(cToken);

            // برگشت دادن نتیجه
            return Result.Success(newTeamResult.Value.Id);
        }
    }
}
