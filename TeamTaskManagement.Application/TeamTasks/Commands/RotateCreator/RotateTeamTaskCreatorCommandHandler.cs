using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.RotateCreator
{
    /// <summary>
    /// اجراکننده دستور تعویض محول کننده وظیفه
    /// </summary>
    internal sealed class RotateTeamTaskCreatorCommandHandler : ICommandHandler<RotateTeamTaskCreatorCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public RotateTeamTaskCreatorCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات تعویض محول کننده وظیفه توسط لیدر
        /// </summary>
        public async Task<Result> Handle
            (RotateTeamTaskCreatorCommand
            request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط منطق اضافی رو بهمراه عملیات سطح دامین اگریگیت
                // به هلپر میدیم، باقی موارد مشترک همونجا مدیریت میشه
                // که در اینجا، عملیات تعویض محول کننده وظیفه توسط
                // لیدر تیم بعنوان پارامتر داده شده
                (team, task, actor) => {

                    // دریافت سطح دسترسی محول کننده جدید
                    var newCreatorRoleResult = team
                        .GetActiveMemberRole(request.NewCreatorMemberId);

                    if (newCreatorRoleResult.IsFailure)
                        return newCreatorRoleResult;

                    var newCreator = new ActingMember
                        (request.NewCreatorMemberId,
                         newCreatorRoleResult.Value);

                    // عملیات دامین اگریگیت تعویض محول کننده وظیفه
                    return task.RotateCreator(actor, newCreator);

                }, ct);
        }
    }
}