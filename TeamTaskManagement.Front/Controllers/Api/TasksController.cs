using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.TeamTasks;

namespace TeamTaskManagement.Web.Controllers.Api {
    /// <summary>
    /// API وظایف تیم
    /// </summary>
    [Route("api/tasks")]
    public sealed class TasksController : ApiControllerBase {
        public TasksController(ISender sender) : base(sender) { }

        /// <summary>
        /// دریافت وظایف من
        /// </summary>
        [HttpGet("for-me")]
        public async Task<IActionResult> GetMyTasksAsync(
            [FromQuery] GetMyTasksRequest request,
            CancellationToken ct)
            => await ExecuteAsync(request.AsQuery(), ct);

        /// <summary>
        /// دریافت وظایف یک تیم
        /// </summary>
        [HttpGet("for-team")]
        public async Task<IActionResult> GetTeamTasksAsync(
            [FromQuery] GetTeamTasksRequest request,
            CancellationToken ct)
            => await ExecuteAsync(request.AsQuery(), ct);

        /// <summary>
        /// دریافت یک وظیفه با شناسه
        /// </summary>
        [HttpGet("{taskId:guid}")]
        public async Task<IActionResult> GetByIdAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsQueryTeamTaskById(), ct);

        /// <summary>
        /// ایجاد وظیفه جدید
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateTeamTaskRequest request,
            CancellationToken ct)
            => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// ویرایش عنوان و توضیحات وظیفه
        /// </summary>
        [HttpPut("details")]
        public async Task<IActionResult> UpdateDetailsAsync(
            [FromBody] UpdateTeamTaskDetailsRequest request,
            CancellationToken ct)
            => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// تغییر ددلاین وظیفه
        /// </summary>
        [HttpPut("deadline")]
        public async Task<IActionResult> UpdateDeadlineAsync(
            [FromBody] UpdateTeamTaskDeadlineRequest request,
            CancellationToken ct)
            => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// تغییر مسئول انجام وظیفه
        /// </summary>
        [HttpPut("assignee")]
        public async Task<IActionResult> RotateAssigneeAsync(
            [FromBody] RotateTeamTaskAssigneeRequest request,
            CancellationToken ct)
            => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// تغییر مدیر/ایجادکننده وظیفه
        /// </summary>
        [HttpPut("creator")]
        public async Task<IActionResult> RotateCreatorAsync(
            [FromBody] RotateTeamTaskCreatorRequest request,
            CancellationToken ct)
            => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// شروع انجام وظیفه
        /// </summary>
        [HttpPost("{taskId:guid}/start")]
        public async Task<IActionResult> StartAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsCommandStartTask(), ct);

        /// <summary>
        /// اعلام تکمیل توسط مسئول
        /// </summary>
        [HttpPost("{taskId:guid}/claim-complete")]
        public async Task<IActionResult> ClaimCompleteAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsCommandClaimComplete(), ct);

        /// <summary>
        /// اعلام شکست توسط مسئول
        /// </summary>
        [HttpPost("{taskId:guid}/claim-failure")]
        public async Task<IActionResult> ClaimFailureAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsCommandClaimFailure(), ct);

        /// <summary>
        /// دستور تلاش مجدد توسط مدیر وظیفه
        /// </summary>
        [HttpPost("{taskId:guid}/order-retry")]
        public async Task<IActionResult> OrderRetryAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsCommandOrderRetry(), ct);

        /// <summary>
        /// پذیرش تلاش مجدد توسط مسئول
        /// </summary>
        [HttpPost("{taskId:guid}/accept-retry")]
        public async Task<IActionResult> AcceptRetryAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsCommandAcceptRetry(), ct);

        /// <summary>
        /// رد تلاش مجدد توسط مسئول
        /// </summary>
        [HttpPost("{taskId:guid}/reject-retry")]
        public async Task<IActionResult> RejectRetryAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsCommandRejectRetry(), ct);

        /// <summary>
        /// بستن وظیفه به عنوان تکمیل شده
        /// </summary>
        [HttpPost("{taskId:guid}/close-complete")]
        public async Task<IActionResult> CloseCompleteAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsCommandCloseComplete(), ct);

        /// <summary>
        /// بستن وظیفه به عنوان شکست خورده
        /// </summary>
        [HttpPost("{taskId:guid}/close-failure")]
        public async Task<IActionResult> CloseFailureAsync(
            Guid taskId,
            CancellationToken ct)
            => await ExecuteAsync(taskId.AsCommandCloseFailure(), ct);
    }
}