using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Users.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Users.Queries.GetCurrentUserProfile
{
    /// <summary>
    /// اجراکننده کوئری دریافت پروفایل کاربر فعلی
    /// </summary>
    internal sealed class GetCurrentUserProfileQueryHandler
        : IQueryHandler<GetCurrentUserProfileQuery, UserResponse> {

        #region Injections

        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetCurrentUserProfileQueryHandler
            (IUserRepository userRepository,
             ICurrentUserService currentUserService) {

            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        #endregion

        public async Task<Result<UserResponse>> Handle
            (GetCurrentUserProfileQuery request, CancellationToken ct) {

            var user = await _userRepository
                .GetByIdAsync(_currentUserService.UserId, ct);

            if (user is null)
                return DomainErrors.User.NotFound.AsResult<UserResponse>();

            var response = new UserResponse
                (user.Id,
                 user.Email.Value,
                 user.Displayname.Value,
                 user.Firstname.Value,
                 user.Lastname.Value,
                 user.CreatedAtUtc,
                 user.LastModifiedAtUtc);

            return response;
        }
    }
}