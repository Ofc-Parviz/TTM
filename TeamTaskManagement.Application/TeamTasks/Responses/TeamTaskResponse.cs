using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Application.Teams.Responses;

namespace TeamTaskManagement.Application.TeamTasks.Responses;

/// <summary>
/// ریسپانس اطلاعات وظیفه
/// </summary>
public sealed record TeamTaskResponse
    (Guid TaskId,
     Guid TeamId,
     string TeamDisplayname,
     string Title,
     string Description,
     TeamTaskStatus Status,
     string StatusText,
     DateTime? DeadlineUtc,
     DateTime CreatedAtUtc,
     DateTime? LastModifiedAtUtc,
     MemberResponse Creator,
     MemberResponse Assignee,
     MyTaskPositionResponse MyPosition,
     MemberRoleResponse MyCurrentAccessLevelInTeam);


/// <summary>
/// ریسپانس نقش کاربر فعلی در وظیفه
/// </summary>
public enum MyTaskPositionResponse {

    /// <summary>
    /// نامشخص
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// بدون نقش
    /// </summary>
    None = 0,

    /// <summary>
    /// مدیر/سازنده وظیفه
    /// </summary>
    Creator = 1,

    /// <summary>
    /// مسئول انجام وظیفه
    /// </summary>
    Assignee = 2,

    /// <summary>
    /// هم مدیر وظیفه هم مسئول انجام آن
    /// </summary>
    Both = 3
}
