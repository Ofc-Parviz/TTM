namespace TeamTaskManagement.Web.Middlewares {
    /// <summary>
    /// اکسپشن هندلینگ برای ای-پی-آی ها
    /// </summary>
    public sealed class ApiExceptionMiddleware {

        #region Injections

        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware
            (RequestDelegate next,
             ILogger<ApiExceptionMiddleware> logger) {

            _next = next;
            _logger = logger;
        }

        #endregion

        public async Task InvokeAsync(HttpContext context) {

            try {
                // ریکوئست رو اجرا میکنیم
                await _next(context);
            }
            // اگر اکسپشن داد
            catch (Exception ex) {

                // چک میکنیم برای ای-پی-آی ها هست یا نه
                if (!context.Request.Path.StartsWithSegments("/api"))
                    throw; // اگر نبود، پرتابش میکنیم تا هندلینگ فرانت بگیرتش

                // اکسپشن رو لاگ میکنیم
                _logger.LogError(ex, "Unhandled API exception");

                if (context.Response.HasStarted) throw;

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var payload = new {
                    code = "Server.UnexpectedError",
                    message = "An unexpected error occurred."
                };

                await context.Response.WriteAsJsonAsync(payload);
            }
        }
    }
}
