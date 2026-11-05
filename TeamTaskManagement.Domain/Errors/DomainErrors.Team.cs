using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Errors
{
    public static partial class DomainErrors {

        /// <summary>
        /// خطا های دامین مربوط به اگریگیت تیم
        /// </summary>
        public static class Team {

            /// <summary>
            /// این کاربر از قبل عضو تیم است
            /// </summary>
            public static readonly Error MemberAlreadyExists = new(
                "Team.MemberAlreadyExists",
                "This user is already a member of the team",
                "این کاربر از قبل عضو تیم است");

            /// <summary>
            /// این عضو در این تیم یافت نشد
            /// </summary>
            public static readonly Error MemberNotFound = new(
                "Team.MemberNotFound",
                "Member not found in this team",
                "این عضو در این تیم یافت نشد");

            /// <summary>
            /// اطلاعات عضویت شما گم شد
            /// </summary>
            public static readonly Error YourMembershipDataIsLost = new(
                "Team.YourMembershipDataIsLost",
                "Your membership data is lost",
                "اطلاعات عضویت شما گم شد");

            /// <summary>
            /// شما عضو این تیم نیستید
            /// </summary>
            public static readonly Error YouAreNotAMemberOfThisTeam = new(
                "Team.YouAreNotAMemberOfThisTeam",
                "You are not a member of this team",
                "شما عضو این تیم نیستید");

            /// <summary>
            /// فرستنده دعوتنامه عضوی از تیم میزبان نیست
            /// </summary>
            public static readonly Error SenderNotMember = new(
                "Team.SenderNotMember",
                "Invite sender is not a member of this team",
                "فرستنده دعوتنامه عضوی از تیم میزبان نیست");

            /// <summary>
            /// این عضو دسترسی مجاز برای اینکار را ندارد
            /// </summary>
            public static readonly Error InsufficientPermission = new(
                "Team.InsufficientPermission",
                "This member does not have permission to perform this action",
                "این عضو دسترسی مجاز برای اینکار را ندارد");

            /// <summary>
            /// یک دعوتنامه معلق برای این کاربر از قبل وجود دارد
            /// </summary>
            public static readonly Error DuplicateInvite = new(
                "Team.DuplicateInvite",
                "A pending invite already exists for this user",
                "یک دعوتنامه معلق برای این کاربر از قبل وجود دارد");

            /// <summary>
            /// دعوتنامه یافت نشد
            /// </summary>
            public static readonly Error InviteNotFound = new(
                "Team.InviteNotFound",
                "Invite not found",
                "دعوتنامه یافت نشد");

            /// <summary>
            /// این دعوتنامه مال این کاربر نیست
            /// </summary>
            public static readonly Error InviteNotOwnedByUser = new(
                "Team.InviteNotOwnedByUser",
                "This invite does not belong to this user",
                "این دعوتنامه مال این کاربر نیست");

            /// <summary>
            /// این عضو فرستنده این دعوتنامه نیست
            /// </summary>
            public static readonly Error InviteNotSendedByMember = new(
                "Team.InviteNotSendedByMember",
                "This invite does not sended by this member",
                "این عضو فرستنده این دعوتنامه نیست");

            /// <summary>
            /// این دعوتنامه دیگر معلق نیست
            /// </summary>
            public static readonly Error InviteNotPending = new(
                "Team.InviteNotPending",
                "This invite is no longer pending",
                "این دعوتنامه دیگر معلق نیست");

            /// <summary>
            /// عضو تیم نمیتواند به خودش دعوتنامه بفرستد
            /// </summary>
            public static readonly Error MemberCannotInviteItself = new(
                "Team.MemberCannotInviteItself",
                "Member can not send invite into himself",
                "عضو تیم نمیتواند به خودش دعوتنامه بفرستد");

            /// <summary>
            /// نمیتوان لیدر را از تیم حذف کرد
            /// </summary>
            public static readonly Error CannotRemoveLeader = new(
                "Team.CannotRemoveLeader",
                "Cannot remove the only leader of the team",
                "نمیتوان لیدر را از تیم حذف کرد");

            /// <summary>
            /// فقط لیدر تیم مجاز به انجام این عملیات است
            /// </summary>
            public static readonly Error LeaderOnlyAction = new(
                "Team.LeaderOnlyAction",
                "Only the team leader is allowed to perform this action",
                "فقط لیدر تیم مجاز به انجام این عملیات است");

            /// <summary>
            /// سطح دسترسی لیدر از طریق این متد قابل تغییر نیست
            /// </summary>
            public static readonly Error LeaderRoleIsImmutable = new(
                "Team.LeaderRoleIsImmutable",
                "The leader's role can not be changed through this action",
                "سطح دسترسی لیدر از طریق این متد قابل تغییر نیست");

            /// <summary>
            /// یک تیم نمیتواند بیش از یک لیدر همزمان داشته باشد
            /// </summary>
            public static readonly Error TeamAlreadyHasLeader = new(
                "Team.TeamAlreadyHasLeader",
                "This team already has a leader; leadership can not be duplicated",
                "یک تیم نمیتواند بیش از یک لیدر همزمان داشته باشد");

            /// <summary>
            /// مدیر میانی نمیتواند برای پوزیشن مدیر میانی دعوتنامه بفرستد
            /// </summary>
            public static readonly Error ManagerCannotInviteOthersInManagerRole = new(
                "Team.ManagerCannotInviteOthersInManagerRole",
                "Manager can not invite other users into manager role in team",
                "ارسال دعوتنامه جهت جذب سرپرست، از اختیارات مالک است. سرپرست فقط برای جذب کارمند میتواند دعوتنامه بفرستد.");

            /// <summary>
            /// لیدر/رئیس نمیتواند تیم را ترک کند
            /// </summary>
            public static readonly Error LeaderCannotLeaveTeam = new(
                "Team.LeaderCannotLeaveTeam",
                "The team-leader cannot leave the team",
                "لیدر/رئیس نمیتواند تیم را ترک کند");

            /// <summary>
            /// این تیم پیدا نشد
            /// </summary>
            public static readonly Error TeamNotFound = new(
                "Team.TeamNotFound",
                "Team information not found",
                "این تیم پیدا نشد");

            public static readonly Error ReceiverUserIdOrEmailIsInvalid = new(
                "Team.ReceiverUserIdOrEmailIsInvalid",
                "Receiver's user-id/email is invalid",
                "ایمیل/شناسه گیرنده نامعتبر است");
        }
    }
}
