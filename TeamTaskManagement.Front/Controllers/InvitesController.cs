using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Application.Teams.Commands.AcceptInvite;
using TeamTaskManagement.Application.Teams.Commands.CancelInvite;
using TeamTaskManagement.Application.Teams.Commands.RejectInvite;
using TeamTaskManagement.Application.Teams.Commands.SendInvite;
using TeamTaskManagement.Application.Teams.Queries.GetMyInvites;
using TeamTaskManagement.Shared.Enums;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.Teams;

namespace TeamTaskManagement.Web.Controllers {
    [Route("Invites")]
    public sealed class InvitesController : MvcControllerBase {
        public InvitesController(ISender sender) : base(sender) { }

        /// <summary>
        /// لیست دعوت‌های من
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            Guid? teamId = null,
            InviteDirectionFilter direction = InviteDirectionFilter.All,
            InviteStatusFilter status = InviteStatusFilter.All,
            CancellationToken ct = default) {
            var result = await Sender.Send(
                new GetMyInvitesQuery(teamId, direction, status), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return View(Array.Empty<object>());
            }

            ViewBag.TeamId = teamId;
            ViewBag.Direction = direction;
            ViewBag.Status = status;

            return View(result.Value);
        }

        /// <summary>
        /// ارسال دعوتنامه
        /// </summary>
        [HttpPost("Send")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(SendTeamInviteRequest request, CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToReferrerOrTeam(request.TeamId);
            }

            TempData["SuccessMessage"] = "دعوتنامه ارسال شد.";
            return RedirectToReferrerOrTeam(request.TeamId);
        }

        /// <summary>
        /// پذیرش دعوتنامه
        /// </summary>
        [HttpPost("Accept")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(AcceptInviteRequest request, CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "دعوتنامه پذیرفته شد.";
            return RedirectToAction("Details", "Teams", new { teamId = request.TeamId });
        }

        /// <summary>
        /// رد دعوتنامه
        /// </summary>
        [HttpPost("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(RejectInviteRequest request, CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "دعوتنامه رد شد.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// لغو دعوتنامه توسط فرستنده/لیدر
        /// </summary>
        [HttpPost("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(CancelInviteRequest request, CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToReferrerOrTeam(request.TeamId);
            }

            TempData["SuccessMessage"] = "دعوتنامه لغو شد.";
            return RedirectToReferrerOrTeam(request.TeamId);
        }

        private IActionResult RedirectToReferrerOrTeam(Guid teamId) {
            var referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrWhiteSpace(referer) &&
                referer.Contains("/Teams/Details", StringComparison.OrdinalIgnoreCase)) {
                return RedirectToAction("Details", "Teams", new { teamId });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}