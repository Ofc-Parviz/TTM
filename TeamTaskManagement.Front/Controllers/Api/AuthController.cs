using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.Auth;

namespace TeamTaskManagement.Web.Controllers.Api {
    /// <summary>
    /// ای-پی-آی ثبت نام و احراز هویت
    /// </summary>
    [Route("api/auth")]
    [AllowAnonymous]
    public sealed class AuthController : ApiControllerBase {
        public AuthController(ISender sender) : base(sender) { }

        /// <summary>
        /// اندپوینت ثبت نام کاربر
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync
            ([FromBody] RegisterRequest request,
             CancellationToken ct) => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// اندپوینت لاگین کاربر
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync
            ([FromBody] LoginRequest request,
             CancellationToken ct) => await ExecuteAsync(request.AsCommand(), ct);
    }
}
