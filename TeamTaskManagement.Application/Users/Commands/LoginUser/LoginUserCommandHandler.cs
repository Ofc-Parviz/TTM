using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Users.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Users.Commands.LoginUser
{
    /// <summary>
    /// اجرا کننده دستور ورود کاربر
    /// </summary>
    internal sealed class LoginUserCommandHandler
        : ICommandHandler<LoginUserCommand, LoginUserResponse> {

        #region Injections

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUserCommandHandler
            (IUserRepository userRepository,
             IPasswordHasher passwordHasher,
             IJwtTokenService jwtTokenService) {

            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        #endregion

        /// <summary>
        /// عملیات ورود کاربر و صدور توکن
        /// </summary>
        public async Task<Result<LoginUserResponse>> Handle
            (LoginUserCommand request, CancellationToken ct) {

            /* از نظر امنیتی بهتره نگیم «ایمیل پیدا نشد» یا «رمز وارد شده 
             * اشتباهه». یه پیام مشترک میدیم تا محاجم نفهمه کدوم غلط بوده */

            // پیدا کردن کاربر با ایمیل
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // اگر کاربر یافت نشد
            if(user is null)
                // میگوییم اطلاعات ورود اشتباه است
                return DomainErrors.User.InvalidCredentials.AsResult<LoginUserResponse>();

            // بررسی رمز عبور خام ورودی با رمز هش شده کاربر
            var verificationSucceed = _passwordHasher
                .Verify(request.Password, user.Password.HashedValue);

            // اگر رمز عبور مطابقت نداشت
            if (!verificationSucceed)
                // باز هم میگوییم اطلاعات ورود اشتباه است
                return DomainErrors.User.InvalidCredentials.AsResult<LoginUserResponse>();

            // اگر همه چیز درست بود، توکن صادر میکنیم
            var token = _jwtTokenService
                .GenerateToken(user.Id, request.Email);

            // و در نهایت ریسپانس لاگین موفق پس میدیم
            return new LoginUserResponse
                (user.Id,
                 request.Email,
                 token.AccessToken,
                 token.ExpiresAtUtc);
        }
    }
}
