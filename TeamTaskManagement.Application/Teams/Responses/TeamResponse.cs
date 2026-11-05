namespace TeamTaskManagement.Application.Teams.Responses;

/// <summary>
/// ریسپانس تیم، برای استفاده در کوئری ها
/// </summary>
public sealed record TeamResponse
    (Guid Id,
     string Displayname,
     string Description,
     DateTime CreatedAtUtc,
     DateTime LastModifiedAtUtc,
     MemberResponse CurrentUserMembership,
     IReadOnlyList<MemberResponse> Members,
     IReadOnlyList<InviteResponse> PendingInvites);