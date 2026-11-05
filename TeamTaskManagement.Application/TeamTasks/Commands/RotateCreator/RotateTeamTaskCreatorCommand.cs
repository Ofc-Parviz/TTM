using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.RotateCreator {
    /// <summary>
    /// دستور تعویض محول کننده وظیفه - فقط توسط لیدر
    /// </summary>
    public sealed record RotateTeamTaskCreatorCommand(
        Guid TeamTaskId,
        Guid NewCreatorMemberId) : ICommand;
}