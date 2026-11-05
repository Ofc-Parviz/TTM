using System.Net;

namespace TeamTaskManagement.Web.Middlewares {
    /// <summary>
    /// اکسپشن هندلینگ سراسری
    /// </summary>
    public sealed class ExceptionHandlingMiddleware {

        #region Injections

        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            IHostEnvironment env,
            ILogger<ExceptionHandlingMiddleware> logger) {

            _next = next;
            _env = env;
            _logger = logger;
        }

        #endregion

        #region Invoker Method

        /// <summary>
        /// متد واسط
        /// </summary>
        public async Task InvokeAsync(HttpContext context) {

            try {
                // ریکوئست رو اجرا میکنیم
                await _next(context);
            }
            catch (Exception ex) {
                // اگر اکسپشن داد، به هندل کننده میدیمش
                await HandleExceptionAsync(context, ex);
            }
        }

        #endregion

        #region Ex-Handler Method

        /// <summary>
        /// هندل کننده اکسپشن در سرتاسر پروژه
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception) {

            // اکسپشن رو لاگ میکنیم
            _logger.LogError(exception, "Unhandled exception");

            // اگر پاسخ از قبل در حال ارسال به کلاینت بود
            if (context.Response.HasStarted)
                // جیغ میزنیم
                throw exception;

            // اگر اکسپشن توی یکی از ای-پی-آی ها رخ داده بود
            if (IsApiRequest(context)) {

                // میدیمش به متد نویسنده خطا برای کلاینت ای-پی-آی
                await WriteApiErrorAsync(context, exception);
                return;
            }

            // در غیر اینصورت، میدیمش به متد نویسنده خطا برای کلاینت وب فرانت
            await WriteMvcErrorAsync(context, exception);
        }

        #endregion

        #region Api Checker Method

        /// <summary>
        /// چک کننده اینکه ریکوئست مال ای-پی-آیه یا نه
        /// </summary>
        private static bool IsApiRequest(HttpContext context)
            => context.Request.Path.StartsWithSegments("/api");

        #endregion

        #region Api Error Writer Method

        /// <summary>
        /// نوشتن ارور برای کلاینت ای-پی-آی
        /// </summary>
        private static async Task WriteApiErrorAsync(HttpContext context, Exception exception) {

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new {
                code = "Server.UnexpectedError",
                message = "An unexpected error occurred.",
                fa_message = "خطای غیرمنتظره ای رخ داد."
            };

            await context.Response.WriteAsJsonAsync(payload);
        }

        #endregion

        #region Mvc Error Writer Method

        /// <summary>
        /// نوشتن ارور برای کلاینت وب فرانت
        /// </summary>
        private async Task WriteMvcErrorAsync(HttpContext context, Exception exception) {

            // ساده‌ترین حالت پایدار: redirect به صفحه خطا
            context.Response.Clear();
            context.Response.Redirect("/Home/Error");
            await Task.CompletedTask;
        }

        #endregion
    }
}
