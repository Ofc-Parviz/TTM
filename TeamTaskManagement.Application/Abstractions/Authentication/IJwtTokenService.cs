using TeamTaskManagement.Application.Users.Responses;

namespace TeamTaskManagement.Application.Abstractions.Authentication;

/// <summary>
/// سرویس تولید و مدیریت JWT
/// </summary>
public interface IJwtTokenService {

    /// <summary>
    /// تولید توکن دسترسی برای کاربر
    /// </summary>
    JwtTokenResponse GenerateToken(Guid userId, string email);
}