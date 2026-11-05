using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Application.Teams.Queries.GetTeamById;
using TeamTaskManagement.Application.TeamTasks.Commands.CreateTeamTask;
using TeamTaskManagement.Application.TeamTasks.Commands.RotateAssignee;
using TeamTaskManagement.Application.TeamTasks.Commands.RotateCreator;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.AcceptRetry;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsComplete;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsFailure;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsComplete;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsFailure;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.OrderRetry;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.RejectRetry;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.StartTeamTask;
using TeamTaskManagement.Application.TeamTasks.Commands.UpdateDeadline;
using TeamTaskManagement.Application.TeamTasks.Commands.UpdateDetails;
using TeamTaskManagement.Application.TeamTasks.Queries.GetMyTasks;
using TeamTaskManagement.Application.TeamTasks.Queries.GetTaskById;
using TeamTaskManagement.Application.TeamTasks.Queries.GetTeamTasks;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.Teams;
using TeamTaskManagement.Web.Contracts.TeamTasks;

namespace TeamTaskManagement.Web.Controllers {
    [Route("Tasks")]
    public sealed class TasksController : MvcControllerBase {
        public TasksController(ISender sender) : base(sender) { }

        /// <summary>
        /// لیست وظایف من
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            [FromQuery] GetMyTasksRequest request,
            CancellationToken ct) {

            var tasks_result = await Sender.Send(request.AsQuery(), ct);

            if (tasks_result.IsFailure) {

                TempData["ErrorMessage"] = tasks_result.Error.Fa_Message ?? tasks_result.Error.Message;
                return View(Array.Empty<object>());
            }

            var teams_result = await Sender.Send(new GetMyTeamsRequest(false).AsQuery(), ct);

            if (teams_result.IsFailure) {

                TempData["ErrorMessage"] = teams_result.Error.Fa_Message ?? teams_result.Error.Message;
                return View(Array.Empty<object>());
            }

            ViewBag.Filter = request;
            ViewData["Teams"] = teams_result.Value;

            return View(tasks_result.Value);
        }

        /// <summary>
        /// وظایف یک تیم — ورود از صفحه جزئیات تیم
        /// </summary>
        [HttpGet("ForTeam")]
        public async Task<IActionResult> ForTeam
            ([FromQuery] GetTeamTasksRequest request, CancellationToken ct) {

            var result = await Sender.Send(request.AsQuery(), ct);

            if (result.IsFailure) {

                TempData["ErrorMessage"] = result.Error
                    .Fa_Message ?? result.Error.Message;
                return RedirectToAction("Index", "Teams");
            }

            return View(result.Value);
        }

        /// <summary>
        /// جزئیات وظیفه
        /// </summary>
        [HttpGet("Details/{taskId:guid}")]
        public async Task<IActionResult> Details(Guid taskId, CancellationToken ct) {
            var result = await Sender.Send(taskId.AsQueryTeamTaskById(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error
                    .Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

        /// <summary>
        /// اعضای یک تیم برای انتخاب مسئول انجام وظیفه (JSON)
        /// </summary>
        [HttpGet("TeamMembers")]
        public async Task<IActionResult> TeamMembers(Guid teamId, CancellationToken ct) {

            var result = await Sender.Send(new GetTeamByIdQuery(teamId), ct);

            if (result.IsFailure) {
                return BadRequest(new { message = result.Error.Fa_Message ?? result.Error.Message });
            }

            var members = result.Value.Members
                .Select(m => new {
                    m.MemberId,
                    m.UserDisplayname,
                    m.UserEmail,
                    IsCurrentUser = m.UserId == result.Value.CurrentUserMembership.UserId
                });

            return Json(members);
        }

        /// <summary>
        /// ایجاد وظیفه
        /// </summary>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamTaskRequest request, CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "وظیفه ایجاد شد.";
            return RedirectToAction(nameof(Details), new { taskId = result.Value });
        }

        /// <summary>
        /// ویرایش عنوان و توضیحات
        /// </summary>
        [HttpPost("UpdateDetails")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDetails(
            UpdateTeamTaskDetailsRequest request,
            CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { taskId = request.TaskId });
            }

            TempData["SuccessMessage"] = "جزئیات وظیفه به‌روزرسانی شد.";
            return RedirectToAction(nameof(Details), new { taskId = request.TaskId });
        }

        /// <summary>
        /// تغییر ددلاین
        /// </summary>
        [HttpPost("UpdateDeadline")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDeadline(
            UpdateTeamTaskDeadlineRequest request,
            CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { taskId = request.TaskId });
            }

            TempData["SuccessMessage"] = "ددلاین به‌روزرسانی شد.";
            return RedirectToAction(nameof(Details), new { taskId = request.TaskId });
        }

        /// <summary>
        /// تغییر مسئول انجام
        /// </summary>
        [HttpPost("RotateAssignee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RotateAssignee(
            RotateTeamTaskAssigneeRequest request,
            CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { taskId = request.TaskId });
            }

            TempData["SuccessMessage"] = "مسئول وظیفه تغییر کرد.";
            return RedirectToAction(nameof(Details), new { taskId = request.TaskId });
        }

        /// <summary>
        /// تغییر مدیر/ایجادکننده وظیفه
        /// </summary>
        [HttpPost("RotateCreator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RotateCreator(
            RotateTeamTaskCreatorRequest request,
            CancellationToken ct) {
            var result = await Sender.Send(request.AsCommand(), ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { taskId = request.TaskId });
            }

            TempData["SuccessMessage"] = "مدیر وظیفه تغییر کرد.";
            return RedirectToAction(nameof(Details), new { taskId = request.TaskId });
        }

        // ── چرخه وضعیت ───────────────────────────────

        [HttpPost("Start/{taskId:guid}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Start(Guid taskId, CancellationToken ct)
            => RunStatusAsync(taskId.AsCommandStartTask(), taskId, "وظیفه شروع شد.", ct);

        [HttpPost("ClaimComplete/{taskId:guid}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ClaimComplete(Guid taskId, CancellationToken ct)
            => RunStatusAsync(taskId.AsCommandClaimComplete(), taskId, "تکمیل اعلام شد.", ct);

        [HttpPost("ClaimFailure/{taskId:guid}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ClaimFailure(Guid taskId, CancellationToken ct)
            => RunStatusAsync(taskId.AsCommandClaimFailure(), taskId, "شکست اعلام شد.", ct);

        [HttpPost("OrderRetry/{taskId:guid}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> OrderRetry(Guid taskId, CancellationToken ct)
            => RunStatusAsync(taskId.AsCommandOrderRetry(), taskId, "دستور تلاش مجدد ثبت شد.", ct);

        [HttpPost("AcceptRetry/{taskId:guid}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> AcceptRetry(Guid taskId, CancellationToken ct)
            => RunStatusAsync(taskId.AsCommandAcceptRetry(), taskId, "تلاش مجدد پذیرفته شد.", ct);

        [HttpPost("RejectRetry/{taskId:guid}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> RejectRetry(Guid taskId, CancellationToken ct)
            => RunStatusAsync(taskId.AsCommandRejectRetry(), taskId, "تلاش مجدد رد شد.", ct);

        [HttpPost("CloseComplete/{taskId:guid}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> CloseComplete(Guid taskId, CancellationToken ct)
            => RunStatusAsync(taskId.AsCommandCloseComplete(), taskId, "وظیفه به‌عنوان تکمیل‌شده بسته شد.", ct);

        [HttpPost("CloseFailure/{taskId:guid}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> CloseFailure(Guid taskId, CancellationToken ct)
            => RunStatusAsync(taskId.AsCommandCloseFailure(), taskId, "وظیفه به‌عنوان شکست‌خورده بسته شد.", ct);

        private async Task<IActionResult> RunStatusAsync<TCommand>(
            TCommand command,
            Guid taskId,
            string successMessage,
            CancellationToken ct)
            where TCommand : IRequest<TeamTaskManagement.Shared.Classes.Result> {
            var result = await Sender.Send(command, ct);

            if (result.IsFailure) {
                TempData["ErrorMessage"] = result.Error.Fa_Message ?? result.Error.Message;
                return RedirectToAction(nameof(Details), new { taskId });
            }

            TempData["SuccessMessage"] = successMessage;
            return RedirectToAction(nameof(Details), new { taskId });
        }
    }
}