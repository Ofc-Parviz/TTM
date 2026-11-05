using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Application.Users.Responses;

namespace TeamTaskManagement.Application.Teams.Responses;

/// <summary>
/// ریسپانس اطلاعات دعوتنامه
/// </summary>
public sealed record InviteResponse
    (Guid InviteId,
     Guid TeamId,
     string TeamDisplayname,
     InviteDirectionResponse Direction,
     TeamInviteStatus Status,
     string StatusText,
     string SuggestedRoleTitle,
     string SuggestedAccessLevel,
     MemberResponse Sender,
     UserSimpleResponse Receiver,
     DateTime CreatedAtUtc,
     DateTime LastModifiedAtUtc);

/// <summary>
/// ریسپانس جهت دعوتنامه نسبت به کاربر لاگین شده
/// </summary>
public enum InviteDirectionResponse {

    /// <summary>
    /// نامشخص
    /// </summary>
    Unknown = -2,

    /// <summary>
    /// دعوتنامه نامعتبر
    /// </summary>
    Invalid = -1,

    /// <summary>
    /// این دعوتنامه ربطی به کاربر ندارد
    /// </summary>
    None = 0,

    /// <summary>
    /// ارسال شده توسط کاربر
    /// </summary>
    SentByUser = 1,

    /// <summary>
    /// دریافت شده توسط کاربر
    /// </summary>
    ReceivedByUser = 2
}
