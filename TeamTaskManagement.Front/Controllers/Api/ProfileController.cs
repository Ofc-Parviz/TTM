using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TeamTaskManagement.Web.Abstractions;
using TeamTaskManagement.Web.Contracts;
using TeamTaskManagement.Web.Contracts.Users;
using TeamTaskManagement.Application.Users.Queries.GetCurrentUserProfile;

namespace TeamTaskManagement.Web.Controllers.Api {
    /// <summary>
    /// ای-پی-آی پروفایل کاربر
    /// </summary>
    [Route("api/profile")]
    public sealed class ProfileController : ApiControllerBase {
        public ProfileController(ISender sender) : base(sender) { }

        /// <summary>
        /// اندپوینت پروفایل من
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyProfileAsync(CancellationToken ct)
            => await ExecuteAsync(new GetCurrentUserProfileQuery(), ct);

        /// <summary>
        /// اندپوینت ویرایش پروفایل من
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateMyProfileAsync
            ([FromBody] UpdateUserProfileRequest request,
             CancellationToken ct) => await ExecuteAsync(request.AsCommand(), ct);

        [HttpPost("new-password")]
        public async Task<IActionResult> UpdatePasswordAsync
            ([FromBody] ChangeUserPasswordRequest request,
             CancellationToken ct) => await ExecuteAsync(request.AsCommand(), ct);
    }
}
