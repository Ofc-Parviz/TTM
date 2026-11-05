using TeamTaskManagement.Application.Teams.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Application.Teams.Queries.GetMyInvites;

/// <summary>
/// کوئری دریافت دعوتنامه‌های مرتبط با کاربر فعلی
/// </summary>
public sealed record GetMyInvitesQuery
    (Guid? TeamId = null,
     InviteDirectionFilter DirectionFilter = InviteDirectionFilter.All,
     InviteStatusFilter StatusFilter = InviteStatusFilter.All)
    : IQuery<IReadOnlyList<InviteResponse>>;