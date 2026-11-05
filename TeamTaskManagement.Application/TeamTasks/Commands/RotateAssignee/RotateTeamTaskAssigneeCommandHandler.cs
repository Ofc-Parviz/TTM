using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.RotateAssignee
{
    /// <summary>
    /// اجراکننده دستور انتقال مسئولیت انجام وظیفه
    /// </summary>
    internal sealed class RotateTeamTaskAssigneeCommandHandler : ICommandHandler<RotateTeamTaskAssigneeCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public RotateTeamTaskAssigneeCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات انتقال مسئولیت انجام وظیفه به عضو دیگر
        /// </summary>
        public async Task<Result> Handle
            (RotateTeamTaskAssigneeCommand
            request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط منطق اضافی رو بهمراه عملیات سطح دامین اگریگیت
                // به هلپر میدیم، باقی موارد مشترک همونجا مدیریت میشه
                // که در اینجا، عملیات تعویض کاربر مسئول وظیفه توسط
                // محول کننده آن وظیفه یا لیدر تیم بعنوان پارامتر داده شده
                (team, task, actor) => {

                    // دریافت سطح دسترسی کاربر مسئول جدید
                    var newAssigneeRoleResult = team
                        .GetActiveMemberRole(request.NewAssigneeMemberId);

                    if (newAssigneeRoleResult.IsFailure)
                        return newAssigneeRoleResult;

                    var newAssignee = new ActingMember
                        (request.NewAssigneeMemberId,
                         newAssigneeRoleResult.Value);

                    // عملیات دامین اگریگیت تعویض کاربر مسئول وظیفه
                    return task.RotateAssignee(actor, newAssignee);

                }, ct);
        }
    }
}