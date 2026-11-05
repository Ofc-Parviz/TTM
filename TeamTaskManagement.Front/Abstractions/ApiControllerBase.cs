using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Web.Abstractions {
    /// <summary>
    /// کلاس پدر همه کنترلر های ای-پی-آی
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = Constants.ApiGroupnameForSwagger)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class ApiControllerBase : ControllerBase {

        /// <summary>
        /// ارسال کننده کوئری/کامند به هندلر مربوطه - از میدییتر
        /// </summary>
        protected readonly ISender Sender;
        protected ApiControllerBase(ISender sender) => Sender = sender;

        /// <summary>
        /// برگشت دادن ریزالت بدون دیتا بعنوان پاسخ
        /// </summary>
        protected IActionResult FromResult(Result result) {

            if (result.IsSuccess) return Ok();

            return MapError(result.Error);
        }

        /// <summary>
        /// برگشت دادن ریزالت دیتا دار بعنوان پاسخ
        /// </summary>
        protected IActionResult FromResult<T>(Result<T> result) {

            if (result.IsSuccess) return Ok(result.Value);

            return MapError(result.Error);
        }

        /// <summary>
        /// مپ کردن خطای برخواسته از ریزالت
        /// </summary>
        private IActionResult MapError(Error error) => error.Code switch {
            "User.InvalidCredentials" => Unauthorized(error),
            "User.NotFound" => NotFound(error),
            "Team.TeamNotFound" => NotFound(error),
            "TeamTask.NotFound" => NotFound(error),
            _ => BadRequest(error)
        };

        /// <summary>
        /// متد اصلی اجرای ریکوئست هایی که ریزالت نتیجه بدون دیتا دارن
        /// </summary>
        protected async Task<IActionResult> ExecuteAsync
            <TResponse>(IRequest<TResponse> request, CancellationToken ct)
            where TResponse : Result {

            var result = await Sender.Send(request, ct);

            return FromResult(result);
        }

        /// <summary>
        /// متد اصلی اجرای ریکوئست هایی که ریزالت نتیجه دیتا دار دارن
        /// </summary>
        protected async Task<IActionResult> ExecuteAsync
            <TResponse>(IRequest<Result<TResponse>> request, CancellationToken ct) {

            var result = await Sender.Send(request, ct);

            return FromResult(result);
        }
    }
}
