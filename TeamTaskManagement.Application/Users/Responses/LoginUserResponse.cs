namespace TeamTaskManagement.Application.Users.Responses;

/// <summary>
/// نتیجه ورود موفق
/// </summary>
public sealed record LoginUserResponse
    (Guid UserId,
     string Email,
     string AccessToken,
     DateTime ExpiresAtUtc);
