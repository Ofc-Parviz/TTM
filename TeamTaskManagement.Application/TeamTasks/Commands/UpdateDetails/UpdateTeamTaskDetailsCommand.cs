using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.UpdateDetails {
    /// <summary>
    /// دستور ویرایش عنوان و توضیحات وظیفه
    /// </summary>
    public sealed record UpdateTeamTaskDetailsCommand(
        Guid TeamTaskId,
        string NewTitle,
        string NewDescription) : ICommand;
}