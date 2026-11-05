
namespace TeamTaskManagement.Persistence.Constants {
    /// <summary>
    /// نام های جداول در دیتابیس
    /// </summary>
    internal static class TableNames {

        /// <summary>
        /// نام جدول کاربران
        /// </summary>
        internal const string Users = nameof(Users);

        /// <summary>
        /// نام جدول تیم ها
        /// </summary>
        internal const string Teams = nameof(Teams);

        /// <summary>
        /// نام جدول وظایف محوله
        /// </summary>
        internal const string TeamTasks = nameof(TeamTasks);

        /// <summary>
        /// نام جدول دعوتنامه های تیم ها
        /// </summary>
        internal const string TeamInvites = nameof(TeamInvites);

        /// <summary>
        /// نام جدول عضو های تیم ها
        /// </summary>
        internal const string TeamMembers = nameof(TeamMembers);
    }
}
