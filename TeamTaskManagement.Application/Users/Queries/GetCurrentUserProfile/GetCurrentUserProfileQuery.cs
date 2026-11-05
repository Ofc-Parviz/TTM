using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Users.Responses;

namespace TeamTaskManagement.Application.Users.Queries.GetCurrentUserProfile;

/// <summary>
/// کوئری دریافت پروفایل کاربر فعلی
/// </summary>
public sealed record GetCurrentUserProfileQuery() : IQuery<UserResponse>;