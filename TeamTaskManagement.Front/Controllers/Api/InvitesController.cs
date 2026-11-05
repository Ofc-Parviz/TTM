using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.Teams;

namespace TeamTaskManagement.Web.Controllers.Api {
    [Route("api/invites")]
    public sealed class InvitesController : ApiControllerBase {
        public InvitesController(ISender sender) : base(sender) { }

        /// <summary>
        /// دریافت دعوتنامه‌های من
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyInvitesAsync
            ([FromQuery] GetMyInvitesRequest request, CancellationToken ct)
             => await ExecuteAsync(request.AsQuery(), ct);

        /// <summary>
        /// ارسال دعوتنامه به کاربر
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendInviteAsync
            ([FromBody] SendTeamInviteRequest request, CancellationToken ct)
             => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// پذیرش دعوتنامه
        /// </summary>
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptInviteAsync
            ([FromBody] AcceptInviteRequest request, CancellationToken ct)
             => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// رد دعوتنامه
        /// </summary>
        [HttpPut("reject")]
        public async Task<IActionResult> RejectInviteAsync
            ([FromBody] RejectInviteRequest request, CancellationToken ct)
             => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// لغو دعوتنامه توسط فرستنده
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> CancelInviteAsync
            ([FromBody] CancelInviteRequest request, CancellationToken ct)
             => await ExecuteAsync(request.AsCommand(), ct);
    }
}
