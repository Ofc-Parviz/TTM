namespace TeamTaskManagement.Shared.Enums;

/// <summary>
/// فیلتر وضعیت دعوتنامه
/// </summary>
public enum InviteStatusFilter {

    /// <summary>
    /// دعوتنامه های معلق
    /// </summary>
    Pending = -1,

    /// <summary>
    /// دعوتنامه های غیر معلق
    /// </summary>
    NotPending = 0,

    /// <summary>
    /// پذیرفته شده ها
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// رد شده ها
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// لغو شده ها
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// اتوماتیک لغو شده ها
    /// </summary>
    AutoCancelled = 4,

    /// <summary>
    /// همه
    /// </summary>
    All = 5

}
