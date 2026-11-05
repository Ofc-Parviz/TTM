
namespace TeamTaskManagement.Domain.Enums {
    /// <summary>
    /// وضعیت دعوتنامه عضویت در تیم
    /// </summary>
    public enum TeamInviteStatus {

        /// <summary>
        /// دعوتنامه فرستاده شد
        /// </summary>
        Sent = 0,

        /// <summary>
        /// دعوتنامه توسط دعوت شونده پذیرفته شد
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// دعوتنامه توسط دعوت شونده رد شد
        /// </summary>
        Rejected = 2,

        /// <summary>
        /// دعوتنامه توسط فرستنده لغو شد
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// دعوتنامه توسط سیستم نامعتبر تشخیص داده و بطور خودکار لغو شد
        /// </summary>
        AutoCancelled = 4
    }
}
