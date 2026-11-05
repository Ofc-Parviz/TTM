using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.CreateTeamTask {
    /// <summary>
    /// دستور ایجاد وظیفه جدید در تیم
    /// </summary>
    public sealed record CreateTeamTaskCommand(
        Guid TeamId,
        Guid AppointedToMemberId,
        string Title,
        string Description,
        DateTime? DeadlineUtc) : ICommand<Guid>;
}