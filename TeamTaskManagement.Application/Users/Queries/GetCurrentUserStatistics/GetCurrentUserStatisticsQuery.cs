using TeamTaskManagement.Application.Users.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Users.Queries.GetCurrentUserStatistics;

/// <summary>
/// کوئری دریافت آمار کاربر فعلی
/// </summary>
public sealed record GetCurrentUserStatisticsQuery() : IQuery<UserStatisticsResponse>;
// بعدا تکمیل میکنم اینو