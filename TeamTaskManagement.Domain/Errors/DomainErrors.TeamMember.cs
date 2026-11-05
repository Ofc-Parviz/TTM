using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Errors
{
    public static partial class DomainErrors {

        /// <summary>
        /// خطا های دامین مربوط به عضو تیم
        /// </summary>
        public static class TeamMember {

            /// <summary>
            /// شناسه کاربر نمیتواند خالی باشد
            /// </summary>
            public static readonly Error UserIdEmpty = new(
                "TeamMember.UserIdEmpty",
                "UserId can not be empty",
                "شناسه کاربر نمیتواند خالی باشد");

            /// <summary>
            /// عضو تیمی که وظیفه باز دارد نمیتواند از تیم حذف شود یا تیم را ترک کند
            /// </summary>
            public static readonly Error HasOpenTasks = new(
                "TeamMember.HasOpenTasks",
                "This member has open tasks and cannot leave or be removed from the team",
                "عضو تیمی که وظیفه باز دارد نمیتواند از تیم حذف شود یا تیم را ترک کند");
        }
    }
}
