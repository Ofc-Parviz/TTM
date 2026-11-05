namespace TeamTaskManagement.Application.Users.Responses;

/// <summary>
/// ریسپانس توکن دسترسی
/// </summary>
public sealed record JwtTokenResponse
    (string AccessToken,
     DateTime ExpiresAtUtc);
