using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Shared.Enums;
using static TeamTaskManagement.Shared.Classes.Constants;

namespace TeamTaskManagement.Domain.Shared
{
    public static class ExtentionMethods {


        public static string MembershipStatusToString(this TeamMemberStatus status) {

            switch (status) {

                case TeamMemberStatus.Active: return "فعال";

                case TeamMemberStatus.Leaved: return "رفت";

                case TeamMemberStatus.Removed: return "حذف شد";

                default: return Unknown;
            }
        }

        public static string MemberAccessLevelRoleToString(this TeamMemberRole role) {

            switch (role) {

                case TeamMemberRole.Member: return "کارمند";

                case TeamMemberRole.Manager: return "سرپرست";

                case TeamMemberRole.Leader: return "مالک";

                default: return Unknown;
            }
        }

        public static string TeamTaskStatusToString(this TeamTaskStatus status) {

            switch (status) {

                case TeamTaskStatus.Pending: return "در صف انتظار برای رسیدگی";

                case TeamTaskStatus.OnGoing: return "در حال انجام";

                case TeamTaskStatus.ClaimedAsComplete: return "اعلام شده بعنوان تکمیل";

                case TeamTaskStatus.ClaimedAsFailure: return "اعلام شده بعنوان شکست";

                case TeamTaskStatus.OrderedToTryAgain: return "فرمان داده شده برای تلاش مجدد";

                case TeamTaskStatus.AcceptedToTryAgain: return "موافقت شده برای انجام تلاش مجدد";

                case TeamTaskStatus.RejectedToTryAgain: return "مخالفت شده برای انجام تلاش مجدد";

                case TeamTaskStatus.ClosedAsFailure: return "پایان یافته بعنوان شکست خورده";

                case TeamTaskStatus.ClosedAsDeadlineFailure: return "پایان یافته بعنوان شکست در ددلاین";

                case TeamTaskStatus.ClosedAsComplete: return "پایان یافته بعنوان تکمیل شده";

                default: return Unknown;
            }
        }

        public static string InviteStatusToString(this TeamInviteStatus status) {

            switch (status) {

                case TeamInviteStatus.Sent: return "فرستاده شد";

                case TeamInviteStatus.Accepted: return "پذیرفته شد";

                case TeamInviteStatus.Rejected: return "رد شد";

                case TeamInviteStatus.Cancelled: return "لغو شد";

                case TeamInviteStatus.AutoCancelled: return "توسط سیستم لغو شد";

                default: return Unknown;
            }
        }

        public static bool ItsClosedTaskStatus(this TeamTaskStatus status)
            => status
                is TeamTaskStatus.ClosedAsFailure
                or TeamTaskStatus.ClosedAsDeadlineFailure
                or TeamTaskStatus.ClosedAsComplete;

        public static bool ItsOpenTaskStatus(this TeamTaskStatus status)
            => !status.ItsClosedTaskStatus();


        public static TeamMemberRole ToDomainVersionEnum(this TeamMemberRoleInput input) {

            switch (input) {

                case TeamMemberRoleInput.Leader: return TeamMemberRole.Leader;

                case TeamMemberRoleInput.Manager: return TeamMemberRole.Manager;

                case TeamMemberRoleInput.Member: return TeamMemberRole.Member;

                default: return TeamMemberRole.Member;// (TeamMemberRole)(int)input;
            }
        }
    }
}
