namespace TeamTaskManagement.Application.Users.Responses;

/// <summary>
/// ریسپانس اطلاعات نمایشی کاربر
/// </summary>
public sealed record UserSimpleResponse
    (Guid Id,
     string Email,
     string Displayname);
