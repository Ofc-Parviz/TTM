
namespace TeamTaskManagement.Domain.Enums {
    /// <summary>
    /// وضعیت عضویت عضو تیم
    /// </summary>
    public enum TeamMemberStatus {

        /// <summary>
        /// عضو فعال تیم
        /// </summary>
        Active,

        /// <summary>
        /// از تیم خارج شده - لفت داده
        /// </summary>
        Leaved,

        /// <summary>
        /// توسط لیدر از تیم حذف شده
        /// </summary>
        Removed
    }
}
