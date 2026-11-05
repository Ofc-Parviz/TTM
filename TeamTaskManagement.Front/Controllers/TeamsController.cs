using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Application.Teams.Commands.CreateTeam;
using TeamTaskManagement.Application.Teams.Commands.LeaveTeam;
using TeamTaskManagement.Application.Teams.Commands.UpdateTeam;
using TeamTaskManagement.Application.Teams.Queries.GetMyTeams;
using TeamTaskManagement.Application.Teams.Queries.GetTeamById;
using TeamTaskManagement.Application.Teams.Commands.RemoveMember;
using TeamTaskManagement.Application.Teams.Commands.ChangeMemberRole;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.Teams;

namespace TeamTaskManagement.Web.Controllers {
    [Route("Teams")]
    public sealed class TeamsController : MvcControllerBase {
        public TeamsController(ISender sender) : base(sender) { }

        /// <summary>
        /// لیست تیم‌های من
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(CancellationToken ct) {

            var result = await Sender.Send(new GetMyTeamsQuery(OnlyCurrentActiveTeams: true), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return View(Array.Empty<MyTeamResponse>());
            }

            return View(result.Value);
        }

        /// <summary>
        /// جزئیات یک تیم
        /// </summary>
        [HttpGet("Details/{teamId:guid}")]
        public async Task<IActionResult> Details(Guid teamId, CancellationToken ct) {

            var result = await Sender.Send(new GetTeamByIdQuery(teamId), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

        /// <summary>
        /// ایجاد تیم جدید
        /// </summary>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamRequest request, CancellationToken ct) {

            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "تیم با موفقیت ایجاد شد.";
            return RedirectToAction(nameof(Details), new { teamId = result.Value });
        }

        /// <summary>
        /// ویرایش اطلاعات تیم
        /// </summary>
        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateTeamRequest request, CancellationToken ct) {

            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { teamId = request.TeamId });
            }

            TempData["SuccessMessage"] = "اطلاعات تیم به‌روزرسانی شد.";
            return RedirectToAction(nameof(Details), new { teamId = request.TeamId });
        }

        /// <summary>
        /// ترک تیم
        /// </summary>
        [HttpPost("Leave/{teamId:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(Guid teamId, CancellationToken ct) {

            var result = await Sender.Send(new LeaveTeamCommand(teamId), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { teamId });
            }

            TempData["SuccessMessage"] = "از تیم خارج شدید.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// حذف عضو از تیم
        /// </summary>
        [HttpPost("{teamId:guid}/remove-member")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember
            (Guid teamId, Guid memberId, CancellationToken ct) {

            var request = new RemoveTeamMemberRequest(teamId, memberId);

            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { teamId = request.TeamId });
            }

            TempData["SuccessMessage"] = "عضو از تیم حذف شد.";
            return RedirectToAction(nameof(Details), new { teamId = request.TeamId });
        }

        /// <summary>
        /// تغییر نقش و سطح دسترسی عضو
        /// </summary>
        [HttpPost("ChangeMemberRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeMemberRole
            (ChangeMemberRoleRequest request, CancellationToken ct) {

            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { teamId = request.TeamId });
            }

            TempData["SuccessMessage"] = "نقش عضو به‌روزرسانی شد.";
            return RedirectToAction(nameof(Details), new { teamId = request.TeamId });
        }
    }
}