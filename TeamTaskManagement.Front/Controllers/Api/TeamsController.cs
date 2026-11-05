using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.Teams;

namespace TeamTaskManagement.Web.Controllers.Api {
    /// <summary>
    /// ای-پی-آی تیم
    /// </summary>
    [Route("api/teams")]
    public sealed class TeamsController : ApiControllerBase {
        public TeamsController(ISender sender) : base(sender) { }

        /// <summary>
        /// دریافت تیم های من
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyTeamsAsync(CancellationToken ct)
            => await ExecuteAsync(new GetMyTeamsRequest(true).AsQuery(), ct);

        /// <summary>
        /// دریافت اطلاعات یک تیم خاص
        /// </summary>
        [HttpGet("{teamId:guid}")]
        public async Task<IActionResult> GetTeamByIdAsync
            (Guid teamId, CancellationToken ct)
             => await ExecuteAsync(teamId.AsQueryTeamById(), ct);

        /// <summary>
        /// ایجاد تیم جدید
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNewTeam
            ([FromBody] CreateTeamRequest request, CancellationToken ct)
             => await ExecuteAsync(request.AsCommand(), ct);


        /// <summary>
        /// ویرایش اطلاعات تیم
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateTeamAsync
            ([FromBody] UpdateTeamRequest request, CancellationToken ct)
             => await ExecuteAsync(request.AsCommand(), ct);

        /// <summary>
        /// ترک تیم
        /// </summary>
        [HttpDelete("{teamId:guid}/leave")]
        public async Task<IActionResult> LeaveTeamAsync
            (Guid teamId, CancellationToken ct)
             => await ExecuteAsync(teamId.AsCommandLeaveTeam(), ct);

        /// <summary>
        /// حذف عضو از تیم
        /// </summary>
        [HttpDelete("{teamId:guid}/members/{memberId:guid}")]
        public async Task<IActionResult> RemoveMemberAsync(
            Guid teamId,
            Guid memberId,
            CancellationToken ct)
            => await ExecuteAsync(new RemoveTeamMemberRequest(teamId, memberId).AsCommand(), ct);

        /// <summary>
        /// تغییر نقش و سطح دسترسی عضو
        /// </summary>
        [HttpPut("{teamId:guid}/members/{memberId:guid}/role")]
        public async Task<IActionResult> ChangeMemberRoleAsync(
            Guid teamId,
            Guid memberId,
            [FromBody] ChangeMemberRoleRequest request,
            CancellationToken ct) {
            if (teamId != request.TeamId || memberId != request.MemberId)
                return BadRequest("Id mismatch");

            return await ExecuteAsync(request.AsCommand(), ct);
        }
    }
}
