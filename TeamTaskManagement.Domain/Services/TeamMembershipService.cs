using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Services
{

    /// <summary>
    /// دامین سرویس مسئول عملیات خروج و حذف عضو از تیم - 
    /// این کلاس به عنوان هماهنگ  کننده بین اگریگیت تیم و اگریگیت وظایف عمل میکند،
    /// بدون اینکه مرز هیچ  کدام از آنها نقض شود.
    /// </summary>
    public sealed class TeamMembershipService {

        private readonly ITeamTaskRepository _teamTaskRepository;

        public TeamMembershipService(ITeamTaskRepository teamTaskRepository) {
            _teamTaskRepository = teamTaskRepository;
        }

        /// <summary>
        /// حذف عضو توسط لیدر - عضوی که وظیفه بسته نشده ای دارد قابل حذف نیست
        /// </summary>
        public async Task<Result<TeamMember>> RemoveMemberAsync(
            Team team,
            Guid actorMemberId,
            Guid targetMemberId,
            CancellationToken cancellationToken = default) {

            var hasOpenTasks = await HasOpenTasksAsync(targetMemberId, cancellationToken);
            if (hasOpenTasks)
                return DomainErrors.TeamMember.HasOpenTasks.AsResult<TeamMember>();

            return team.RemoveMember(actorMemberId, targetMemberId);
        }

        /// <summary>
        /// ترک تیم توسط خود عضو - عضوی که وظیفه بسته نشده ای دارد نمیتواند تیم را ترک کند
        /// </summary>
        public async Task<Result<TeamMember>> LeaveTeamAsync(
            Team team,
            Guid memberIdToLeave,
            CancellationToken cancellationToken = default) {

            var hasOpenTasks = await HasOpenTasksAsync(memberIdToLeave, cancellationToken);
            if (hasOpenTasks)
                return DomainErrors.TeamMember.HasOpenTasks.AsResult<TeamMember>();

            return team.LeaveTeam(memberIdToLeave);
        }

        // =======================================

        private async Task<bool> HasOpenTasksAsync(
            Guid memberId,
            CancellationToken cancellationToken) {

            return await _teamTaskRepository
                .HasOpenTasksForMemberAsync(memberId, cancellationToken);
        }
    }
}