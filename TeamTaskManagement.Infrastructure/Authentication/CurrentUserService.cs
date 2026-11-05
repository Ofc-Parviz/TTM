using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using TeamTaskManagement.Application.Abstractions.Authentication;

namespace TeamTaskManagement.Infrastructure.Authentication {
    /// <summary>
    /// خواندن کاربر فعلی از جی-دبلیو-تی داخل HttpContext
    /// </summary>
    internal sealed class CurrentUserService : ICurrentUserService {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public Guid UserId {

            get {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user is null || !IsAuthenticated) return Guid.Empty;

                // اول کلیم سفارشی یو-آی-دی، بعد ساب استاندارد
                var value =
                    user.FindFirst("uid")?.Value ??
                    user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                return Guid.TryParse(value, out var id) ? id : Guid.Empty;
            }
        }

        public string Email {

            get {

                var user = _httpContextAccessor.HttpContext?.User;

                if (user is null || !IsAuthenticated) return string.Empty;

                var value =
                    user.FindFirst(ClaimTypes.Email)?.Value ??
                    user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

                return value ?? string.Empty;
            }
        }
    }
}