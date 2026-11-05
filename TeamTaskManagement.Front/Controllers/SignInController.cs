using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagement.Application.Users.Commands.LoginUser;
using TeamTaskManagement.Application.Users.Responses;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts.Auth;
using TeamTaskManagement.Web.Contracts;

namespace TeamTaskManagement.Web.Controllers {
    [Route("signin")]
    [AllowAnonymous]
    public class SignInController : MvcControllerBase {

        public SignInController(ISender sender) : base(sender) { }

        [HttpGet]
        public IActionResult Index() {

            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Teams");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login
            (LoginRequest request, CancellationToken ct)
             => await ExecuteAsync(request.AsCommand(),
                 onSuccess: async login => {
                     await SignInCookieAsync(login);
                     TempData["SuccessMessage"] = "با موفقیت وارد شدید.";
                     return RedirectToAction("Index", "Teams");
                 },
                 ct, failureAction: nameof(Index));

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register
            (RegisterRequest request, CancellationToken ct) {

            var registerResult = await Sender.Send(request.AsCommand(), ct);

            if (registerResult.IsFailure) {

                TempData["ErrorMessage"] =
                    registerResult.Error.Fa_Message ?? registerResult.Error.Message;

                return RedirectToAction(nameof(Index));
            }

            // لاگین خودکار بعد از ثبت نام موفق
            return await ExecuteAsync
                (new LoginUserCommand(request.Email, request.Password),
                onSuccess: async login => {
                    await SignInCookieAsync(login);
                    TempData["SuccessMessage"] = "ثبت‌نام و ورود با موفقیت انجام شد.";
                    return RedirectToAction("Index", "Teams");
                },
                ct,
                failureAction: nameof(Index));
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout() {

            if (User.Identity?.IsAuthenticated == true) {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["SuccessMessage"] = "با موفقیت خارج شدید.";
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        private async Task SignInCookieAsync(LoginUserResponse login) {

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, login.UserId.ToString()),
                new(ClaimTypes.Email, login.Email),
                new(ClaimTypes.Name, login.Email),
                new("access_token", login.AccessToken)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties {
                    IsPersistent = true,
                    ExpiresUtc = login.ExpiresAtUtc
                });
        }
    }
}
