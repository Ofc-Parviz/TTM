using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Web.Abstractions {
    /// <summary>
    /// کلاس پدر همه کنترلر های فرانت اند ام-وی-سی
    /// </summary>
    [Authorize] // پیشفرض Cookie (DefaultScheme)
    [ApiExplorerSettings(GroupName = Constants.MvcGroupnameForSwagger)]
    public abstract class MvcControllerBase : Controller {

        /// <summary>
        /// ارسال کننده کوئری/کامند به هندلر مربوطه - از میدییتر
        /// </summary>
        protected readonly ISender Sender;
        protected MvcControllerBase(ISender sender) => Sender = sender;


        /// <summary>
        /// نتیجه بدون داده: موفق → Redirect؛ ناموفق → برگشت با TempData خطا
        /// </summary>
        protected IActionResult FromResult(
            Result result,
            string successAction,
            string successController,
            string? successMessage = null,
            string failureAction = "Index") {

            if (result.IsSuccess) {
                if (!string.IsNullOrWhiteSpace(successMessage))
                    TempData["SuccessMessage"] = successMessage;

                return RedirectToAction(successAction, successController);
            }

            TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
            return RedirectToAction(failureAction);
        }

        /// <summary>
        /// نتیجه داده‌دار: معمولاً برای سناریوهایی که بعد از موفقیت باید با Value کاری کرد
        /// </summary>
        protected IActionResult FromResult<T>(
            Result<T> result,
            Func<T, IActionResult> onSuccess,
            string failureAction = "Index") {
            if (result.IsSuccess)
                return onSuccess(result.Value);

            TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
            return RedirectToAction(failureAction);
        }

        protected async Task<IActionResult> ExecuteAsync<TResponse>(
            IRequest<TResponse> request,
            string successAction,
            string successController,
            CancellationToken ct,
            string? successMessage = null,
            string failureAction = "Index")
            where TResponse : Result {
            var result = await Sender.Send(request, ct);
            return FromResult(result, successAction, successController, successMessage, failureAction);
        }

        protected async Task<IActionResult> ExecuteAsync<TResponse>(
            IRequest<Result<TResponse>> request,
            Func<TResponse, Task<IActionResult>> onSuccess,
            CancellationToken ct,
            string failureAction = "Index") {

            var result = await Sender.Send(request, ct);

            if (result.IsSuccess)
                return await onSuccess(result.Value);

            TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;

            return RedirectToAction(failureAction);
        }
    }
}
