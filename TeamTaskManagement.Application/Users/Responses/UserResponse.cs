namespace TeamTaskManagement.Application.Users.Responses;

/// <summary>
/// ریسپانس اطلاعات کاربر
/// </summary>
public sealed record UserResponse
    (Guid Id,
     string Email,
     string Displayname,
     string Firstname,
     string Lastname,
     DateTime CreatedAtUtc,
     DateTime LastModifiedAtUtc);