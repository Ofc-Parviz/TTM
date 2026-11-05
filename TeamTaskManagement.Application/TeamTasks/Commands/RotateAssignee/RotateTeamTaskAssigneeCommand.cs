using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.RotateAssignee {
    /// <summary>
    /// دستور انتقال مسئولیت انجام وظیفه به عضو دیگر
    /// </summary>
    public sealed record RotateTeamTaskAssigneeCommand(
        Guid TeamTaskId,
        Guid NewAssigneeMemberId) : ICommand;
}