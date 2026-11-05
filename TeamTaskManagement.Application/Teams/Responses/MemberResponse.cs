namespace TeamTaskManagement.Application.Teams.Responses;

/// <summary>
/// ریسپانس عضویت درگیر در وظیفه
/// </summary>
public sealed record MemberResponse
    (Guid MemberId,
     Guid UserId,
     string UserEmail,
     string UserDisplayname,
     string MemberRoleDisplayname,
     string MemberAccessLevelText,
     MemberRoleResponse MemberAccessLevel,
     string CurrentMembershipStatusText,
     DateTime JoinedAtUtc,
     DateTime LastMembershipStatusChangeAtUtc);


/// <summary>
/// ریسپانس نقش عضو در تیم
/// </summary>
public enum MemberRoleResponse {

    /// <summary>
    /// مالک (همون لیدر) تیم
    /// </summary>
    Leader,

    /// <summary>
    /// سرپرست (همون مدیر منصوب شده) وظایف
    /// </summary>
    Manager,

    /// <summary>
    /// کارمند - عضو معمولی
    /// </summary>
    Member
}