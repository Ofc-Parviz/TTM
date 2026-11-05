using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.UpdateDetails
{
    /// <summary>
    /// اجراکننده دستور ویرایش عنوان و توضیحات وظیفه
    /// </summary>
    internal sealed class UpdateTeamTaskDetailsCommandHandler : ICommandHandler<UpdateTeamTaskDetailsCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public UpdateTeamTaskDetailsCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات ویرایش عنوان و توضیحات وظیفه
        /// </summary>
        public async Task<Result> Handle
            (UpdateTeamTaskDetailsCommand request, CancellationToken ct) {

            // ایجاد عنوان وظیفه
            var titleResult = TaskTitle.Create(request.NewTitle);
            if (titleResult.IsFailure)
                return titleResult;

            // ایجاد توضیحات وظیفه
            var descriptionResult = TaskDescription.Create(request.NewDescription);
            if (descriptionResult.IsFailure)
                return descriptionResult;

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط عملیات سطح دامین اگریگیت مد نظر رو میفرستیم
                // که در اینجا، ویرایش عنوان و توضیحات وظیفه
                // توسط محول کننده آن وظیفه یا لیدر تیم است
                (task, actor) => task.UpdateDetails(actor,
                 titleResult.Value, descriptionResult.Value), ct);
        }
    }
}