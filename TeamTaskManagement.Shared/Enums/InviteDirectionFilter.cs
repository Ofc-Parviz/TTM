namespace TeamTaskManagement.Shared.Enums;

/// <summary>
/// فیلتر نوع دعوتنامه نسبت به کاربر
/// </summary>
public enum InviteDirectionFilter {

    /// <summary>
    /// دعوتنامه‌هایی که کاربر ارسال کرده
    /// </summary>
    Sent = 0,

    /// <summary>
    /// دعوتنامه‌هایی که کاربر دریافت کرده
    /// </summary>
    Received = 1,

    /// <summary>
    /// همه دعوتنامه‌های مرتبط با کاربر
    /// </summary>
    All = 2
}
