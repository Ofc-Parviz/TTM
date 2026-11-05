
namespace TeamTaskManagement.Application.Enums {
    /// <summary>
    /// نوع ایمیل اعلان گروهی تغییر عضویت های تیم
    /// </summary>
    public enum MembershipUpdateEmailType {

        /// <summary>
        /// اعلان عضو جدید
        /// </summary>
        Added,

        /// <summary>
        /// اعلان حذف شدن عضو
        /// </summary>
        Removed,

        /// <summary>
        /// اعلان ترک تیم توسط عضو
        /// </summary>
        Left
    }
}
