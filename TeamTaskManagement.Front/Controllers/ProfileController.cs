using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Application.Users.Commands.ChangePassword;
using TeamTaskManagement.Application.Users.Commands.UpdateProfile;
using TeamTaskManagement.Application.Users.Queries.GetCurrentUserProfile;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.Users;

namespace TeamTaskManagement.Web.Controllers {
    [Route("Profile")]
    public sealed class ProfileController : MvcControllerBase {
        public ProfileController(ISender sender) : base(sender) { }

        /// <summary>
        /// نمایش پروفایل کاربر فعلی
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(CancellationToken ct) {

            var result = await Sender.Send(new GetCurrentUserProfileQuery(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction("Index", "Teams");
            }

            return View(result.Value);
        }

        /// <summary>
        /// ویرایش نام و نام نمایشی
        /// </summary>
        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateUserProfileRequest request, CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "پروفایل با موفقیت به‌روزرسانی شد.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// تغییر رمز عبور
        /// </summary>
        [HttpPost("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword
            (ChangeUserPasswordRequest request,
             string? confirmNewPassword,
             CancellationToken ct) {

            if (!string.Equals(request.NewPassword,
                confirmNewPassword, StringComparison.Ordinal)) {

                TempData["ErrorMessage"] = "رمز عبور جدید و تکرار آن یکسان نیستند.";
                return RedirectToAction(nameof(Index));
            }

            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "رمز عبور با موفقیت تغییر کرد.";
            return RedirectToAction(nameof(Index));
        }
    }
}