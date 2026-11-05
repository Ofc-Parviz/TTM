using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Errors
{
    public static partial class DomainErrors {

        /// <summary>
        /// خطا های دامین مربوط به وظیفه محول شده
        /// </summary>
        public static class TeamTask {

            /// <summary>
            /// تاریخ ددلاین نمیتواند در گذشته باشد
            /// </summary>
            public static readonly Error DeadlineInPast = new(
                "TeamTask.DeadlineInPast",
                "Deadline can not be in the past",
                "تاریخ ددلاین نمیتواند در گذشته باشد");

            /// <summary>
            /// این تغییر وضعیت وظیفه ممکن نیست
            /// </summary>
            public static readonly Error InvalidTransition = new(
                "TeamTask.InvalidTransition",
                "This status transition is not allowed",
                "این تغییر وضعیت وظیفه ممکن نیست");

            /// <summary>
            /// این عضو دسترسی لازم برای اینکار را ندارد
            /// </summary>
            public static readonly Error Unauthorized = new(
                "TeamTask.Unauthorized",
                "This team-member is not authorized to perform this action",
                "این عضو دسترسی لازم برای اینکار را ندارد");

            /// <summary>
            /// دیگران نمیتوانند به لیدر وظیفه محول کنند
            /// </summary>
            public static readonly Error OthersCannotAssignOrRotateToLeader = new(
                "TeamTask.OthersCannotAssignOrRotateToLeader",
                "Others can not assign or rotate a team-task to the team-leader",
                "دیگران نمیتوانند به لیدر وظیفه محول کنند");

            /// <summary>
            /// عضو معمولی نمیتواند محول کننده وظیفه به دیگران باشد
            /// </summary>
            public static readonly Error MembersCannotBeCreatorOfOthersTasks = new(
                "Team.MembersCannotBeCreatorOfOthersTasks",
                "members with lower role than Manager can not be the creator of others tasks",
                "عضو معمولی نمیتواند مدیر وظیفه دیگران باشد");

            /// <summary>
            /// وظایفی که پرونده آنها بسته شده نمیتوانند به عضو دیگری منتقل شوند
            /// </summary>
            public static readonly Error ClosedTasksCantBeRotated = new(
                "TeamTask.ClosedTasksCantBeRotated",
                "Closed team-task can not rotate to another team-member",
                "وظایفی که پرونده آنها بسته شده نمیتوانند به عضو دیگری منتقل شوند");

            /// <summary>
            /// نمیتوان وظیفه را به عضو یکسان منتقل کرد
            /// </summary>
            public static readonly Error CantRotateTaskIntoSameMember = new(
                "TeamTask.CantRotateTaskIntoSameMember",
                "Can not rotate team-tasks into same team-member",
                "نمیتوان وظیفه را به عضو یکسان منتقل کرد");

            /// <summary>
            /// نمیتوان ددلاین وظیفه ای که پرونده اش بسته شده را تغییر داد
            /// </summary>
            public static readonly Error CantChangeDeadlineOfClosedTask = new(
                "TeamTask.CantChangeDeadlineOfClosedTask",
                "Can not change deadline for a closed team-task",
                "نمیتوان ددلاین وظیفه ای که پرونده اش بسته شده را تغییر داد");

            /// <summary>
            /// نمیتوان عنوان و توضیحات وظیفه ای که پرونده اش بسته شده را تغییر داد
            /// </summary>
            public static readonly Error CantChangeDetailsOfClosedTask = new(
                "TeamTask.CantChangeDetailsOfClosedTask",
                "Can not change details for a closed team-task",
                "نمیتوان عنوان و توضیحات وظیفه ای که پرونده اش بسته شده را تغییر داد");

            /// <summary>
            /// این وظیفه یافت نشد
            /// </summary>
            public static readonly Error TaskNotFound = new(
                "TeamTask.NotFound",
                "team-task not found",
                "این وظیفه یافت نشد");
        }
    }
}
