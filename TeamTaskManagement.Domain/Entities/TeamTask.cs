using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Shared.Classes;
using TeamTaskManagement.Domain.Shared;

namespace TeamTaskManagement.Domain.Entities
{
    /// <summary>
    /// اگریگیت وظیفه محول شده به عضو تیم
    /// </summary>
    public class TeamTask : AggregateRoot {

        #region Internal Constructor

        /// <summary>
        /// کانستراکتور داخلی
        /// </summary>
        private TeamTask(
            Guid id,
            Guid teamId,
            Guid appointedToMemberId,
            Guid createdByMemberId,
            TaskTitle taskTitle,
            TaskDescription taskDescription,
            DateTime? deadlineUtc = null) : base(id) {

            TeamId = teamId;
            AppointedToMemberId = appointedToMemberId;
            CreatedByMemberId = createdByMemberId;
            Title = taskTitle;
            Description = taskDescription;
            Status = TeamTaskStatus.Pending;
            DeadlineUtc = deadlineUtc;
        }

        /// <summary>
        /// کانستراکتور برای ای-اف-کور تا بتونه مایگرشن بزنه
        /// </summary>
        #pragma warning disable CS8618
        private TeamTask() : base(Guid.Empty) { }

        #endregion

        #region Domain Properties

        /// <summary>
        /// شناسه تیم مرتبط
        /// </summary>
        public Guid TeamId { get; private set; }

        /// <summary>
        /// عضوی از تیم که وظیفه بهش محول شده
        /// </summary>
        public Guid AppointedToMemberId { get; private set; }

        /// <summary>
        /// عضوی از تیم که وظیفه رو به خودش یا عضو دیگری محول کرده
        /// </summary>
        public Guid CreatedByMemberId { get; private set; }

        /// <summary>
        /// عنوان وظیفه محول شده
        /// </summary>
        public TaskTitle Title { get; private set; }

        /// <summary>
        /// توضیحات وظیفه محول شده
        /// </summary>
        public TaskDescription Description { get; private set; }

        /// <summary>
        /// وضعیت وظیفه محول شده
        /// </summary>
        public TeamTaskStatus Status { get; private set; }

        /// <summary>
        /// ددلاین - آخرین مهلت
        /// </summary>
        public DateTime? DeadlineUtc { get; private set; }

        #endregion

        #region Domain-Method: Create

        /// <summary>
        /// ایجاد وظیفه جدید - فقط لیدر یا مدیرمیانی مجازند
        /// - دیگران نمیتواند به لیدر وظیفه محول کند
        /// </summary>
        public static Result<TeamTask> Create(
            Guid teamId,
            ActingMember creator,
            ActingMember assignee,
            TaskTitle title,
            TaskDescription description,
            DateTime? deadlineUtc = null) {


            var itsNotSelfTasking = creator.MemberId != assignee.MemberId;

            var creatorRoleIsMember = creator.Role == TeamMemberRole.Member;

            var assigneeIsLeader = assignee.Role == TeamMemberRole.Leader;


            // عضو معمولی حق تعیین وظیفه برای دیگران را ندارد
            if (creatorRoleIsMember && itsNotSelfTasking) {

                return DomainErrors.TeamTask.Unauthorized.AsResult<TeamTask>();
            }

            // دیگران حق تعیین وظیفه برای لیدر را ندارند
            if (assigneeIsLeader && itsNotSelfTasking) {

                return DomainErrors.TeamTask.OthersCannotAssignOrRotateToLeader.AsResult<TeamTask>();
            }

            // هنگام ایجاد وظیفه، تاریخ ددلاین نباید در گذشته باشد
            if (deadlineUtc is not null && deadlineUtc <= DateTime.UtcNow) {

                return DomainErrors.TeamTask.DeadlineInPast.AsResult<TeamTask>();
            }

            var newTask = new TeamTask(Guid.NewGuid(), teamId, assignee.MemberId,
                creator.MemberId, title, description, deadlineUtc);

            newTask.RaiseDomainEvent(
                new TeamTaskCreatedEvent(newTask.Id,
                                         teamId,
                                         assignee.MemberId,
                                         creator.MemberId,
                                         title.Value,
                                         newTask.DeadlineUtc));

            return newTask;
        }

        #endregion

        #region Domain-Method: Update Details

        /// <summary>
        /// ویرایش جزییات وظیفه - فقط توسط محول کننده یا لیدر
        /// </summary>
        public Result UpdateDetails
            (ActingMember actor,
             TaskTitle title,
             TaskDescription description) {

            // عضو معمولی یا مدیر میانی سابق نمیتواند وظیفه دیگران را ویرایش کند
            var authResult = EnsureIsManagerCreatorOrLeaderOrSelfTasker(actor);
            if (authResult.IsFailure) return authResult;

            if (Status.ItsClosedTaskStatus())
                return DomainErrors.TeamTask.CantChangeDetailsOfClosedTask.AsResult();

            Title = title;
            Description = description;
            LastModifiedAtUtc = DateTime.UtcNow;

            RaiseDomainEvent(new TeamTaskDetailsUpdatedEvent(
                Id,
                TeamId,
                actor.MemberId,
                AppointedToMemberId,
                actor.Role is TeamMemberRole.Leader,
                title.Value,
                description.Value));

            return Result.Success();
        }

        #endregion

        #region Domain-Method: Update Deadline

        /// <summary>
        /// تغییر ددلاین - فقط توسط لیدر یا مدیر میانی یا خویش مسئول
        /// </summary>
        public Result UpdateDeadline(ActingMember actor, DateTime? newDeadlineUtc) {

            var authResult = EnsureIsManagerCreatorOrLeaderOrSelfTasker(actor);
            if (authResult.IsFailure) return authResult;

            // تاریخ ددلاین جدید نباید در گذشته باشد
            if (newDeadlineUtc is not null &&
                newDeadlineUtc <= DateTime.UtcNow)
                return DomainErrors.TeamTask.DeadlineInPast.AsResult();

            var taskAlreadyClosed = Status is TeamTaskStatus.ClosedAsComplete
                or TeamTaskStatus.ClosedAsFailure or TeamTaskStatus.ClosedAsDeadlineFailure;

            // وظیفه ای که پرونه اش بسته شده را نمیتوان تغییر ددلاین داد
            if (taskAlreadyClosed)
                return DomainErrors.TeamTask.CantChangeDeadlineOfClosedTask.AsResult();


            var oldDeadlineUtc = DeadlineUtc;
            DeadlineUtc = newDeadlineUtc;
            LastModifiedAtUtc = DateTime.UtcNow;

            RaiseDomainEvent(new TeamTaskDeadlineChangedEvent(
                Id,
                TeamId,
                actor.MemberId,
                AppointedToMemberId,
                actor.Role is TeamMemberRole.Leader,
                Title.Value,
                oldDeadlineUtc,
                newDeadlineUtc));

            return Result.Success();
        }

        #endregion

        #region Domain-Method: Rotate Creator

        /// <summary>
        /// تعویض کاربر محول کننده وظیفه - فقط لیدر
        /// </summary>
        public Result RotateCreator(ActingMember actor, ActingMember newCreator) {

            var authResult = EnsureIsLeader(actor);
            if (authResult.IsFailure) return authResult;


            var taskAlreadyClosed = Status is TeamTaskStatus.ClosedAsComplete
                or TeamTaskStatus.ClosedAsFailure or TeamTaskStatus.ClosedAsDeadlineFailure;

            // وظیفه ای که پرونه اش بسته شده را نمیتوان به محول کننده دیگری انتقال داد
            if (taskAlreadyClosed)
                return DomainErrors.TeamTask.ClosedTasksCantBeRotated.AsResult();

            // نمیتوان وظیفه را به محول کننده یکسان منتقل کرد
            var itsNotNewCreator = CreatedByMemberId == newCreator.MemberId;

            if (itsNotNewCreator)
                return DomainErrors.TeamTask.CantRotateTaskIntoSameMember.AsResult();

            var itsNotSelfTaskingConvertion = newCreator.MemberId != AppointedToMemberId;

            var newCreatorIsMember = newCreator.Role is TeamMemberRole.Member;

            // نمیتوان عضو با سطح دسترسی معمولی رو
            // در جایگاه محول کننده وظیفه بر دیگران منصوب کرد
            if (newCreatorIsMember && itsNotSelfTaskingConvertion)
                return DomainErrors.TeamTask.MembersCannotBeCreatorOfOthersTasks.AsResult();

            var oldCreatorMemberId = CreatedByMemberId;

            CreatedByMemberId = newCreator.MemberId;

            LastModifiedAtUtc = DateTime.UtcNow;

            RaiseDomainEvent(new TeamTaskCreatorRotatedEvent
                (Id,
                 TeamId,
                 actor.MemberId,
                 oldCreatorMemberId,
                 newCreator.MemberId,
                 AppointedToMemberId,
                 Title.Value));

            return Result.Success();
        }

        #endregion

        #region Domain-Method: Rotate Assignee

        /// <summary>
        /// سپردن اجرای وظیفه به عضو دیگر تیم - فقط محول کننده یا لیدر
        /// </summary>
        public Result RotateAssignee(ActingMember actor, ActingMember newAssignee) {

            var authResult = EnsureIsManagerCreatorOrLeader(actor);
            if (authResult.IsFailure) return authResult;

            var taskAlreadyClosed = Status is TeamTaskStatus.ClosedAsComplete
                or TeamTaskStatus.ClosedAsFailure or TeamTaskStatus.ClosedAsDeadlineFailure;

            // وظیفه ای که پرونه اش بسته شده را نمیتوان به مسئول دیگری انتقال داد
            if (taskAlreadyClosed)
                return DomainErrors.TeamTask.ClosedTasksCantBeRotated.AsResult();

            // نمیتوان وظیفه را به عضو یکسان منتقل کرد
            var itsNotNewAsignee = AppointedToMemberId == newAssignee.MemberId;
            if (itsNotNewAsignee)
                return DomainErrors.TeamTask.CantRotateTaskIntoSameMember.AsResult();

            var itsNotSelfTasking = actor.MemberId != newAssignee.MemberId;

            var newAssigneeIsLeader = newAssignee.Role is TeamMemberRole.Leader;

            // دیگران نمیتوانند وظیفه ای به لیدر انتقال دهند
            if (itsNotSelfTasking && newAssigneeIsLeader)
                return DomainErrors.TeamTask.OthersCannotAssignOrRotateToLeader.AsResult();

            var oldAssigneeMemberId = AppointedToMemberId;

            AppointedToMemberId = newAssignee.MemberId;

            LastModifiedAtUtc = DateTime.UtcNow;

            RaiseDomainEvent(new TeamTaskAssigneeRotatedEvent(
                Id,
                TeamId,
                actor.MemberId,
                oldAssigneeMemberId,
                newAssignee.MemberId, Title.Value));

            SetStatus(TeamTaskStatus.Pending, actor);

            return Result.Success();
        }

        #endregion

        #region Domain-Method: Start Team-Task

        /// <summary>
        /// شروع وظیفه - فقط توسط عضو مسئول (assignee)
        /// </summary>
        public Result Start(ActingMember assignee) {

            var authResult = EnsureIsAssignee(assignee);
            if (authResult.IsFailure) return authResult;

            return TransitionTo
                (TeamTaskStatus.OnGoing,
                TeamTaskStatus.Pending, assignee);
        }

        #endregion

        #region Domain-Method: Claim as Complete

        /// <summary>
        /// اعلام انجام موفق وظیفه - فقط توسط عضو مسئول (assignee)
        /// </summary>
        public Result ClaimComplete(ActingMember assignee) {

            var authResult = EnsureIsAssignee(assignee);
            if (authResult.IsFailure) return authResult;

            return TransitionTo
                (assignee, TeamTaskStatus.ClaimedAsComplete,
                 TeamTaskStatus.OnGoing, TeamTaskStatus.AcceptedToTryAgain);
        }

        #endregion

        #region Domain-Method: Claim as Failure

        /// <summary>
        /// اعلام شکست در انجام وظیفه - فقط توسط عضو مسئول (assignee)
        /// </summary>
        public Result ClaimFailure(ActingMember assignee) {

            var authResult = EnsureIsAssignee(assignee);
            if (authResult.IsFailure) return authResult;

            return TransitionTo
                (assignee, TeamTaskStatus.ClaimedAsFailure,
                 TeamTaskStatus.OnGoing, TeamTaskStatus.AcceptedToTryAgain);
        }

        #endregion

        #region Domain-Method: Order to Retry

        /// <summary>
        /// دستور تلاش مجدد - فقط توسط عضو محول کننده یا لیدر (creator)
        /// </summary>
        public Result OrderRetry(ActingMember actor) {

            var authResult = EnsureIsManagerCreatorOrLeader(actor);
            if (authResult.IsFailure) return authResult;

            // نمیتوان برای وظیفه ای که در یکی از وضعیت
            // های «اعلام تکمیل» یا «اعلام شکست» نیست دستور تلاش مجدد صادر کرد
            if (Status is not (TeamTaskStatus
                .ClaimedAsComplete or TeamTaskStatus.ClaimedAsFailure))
                return DomainErrors.TeamTask.InvalidTransition.AsResult();

            return SetStatus(TeamTaskStatus.OrderedToTryAgain, actor);
        }

        #endregion

        #region Domain-Method: Accept Retry

        /// <summary>
        /// پذیرفتن دستور تلاش مجدد - فقط توسط عضو مسئول (assignee)
        /// </summary>
        public Result AcceptRetry(ActingMember assignee) {

            var authResult = EnsureIsAssignee(assignee);
            if (authResult.IsFailure) return authResult;

            return TransitionTo
                (TeamTaskStatus.AcceptedToTryAgain,
                 TeamTaskStatus.OrderedToTryAgain, assignee);
        }

        #endregion

        #region Domain-Method: Reject Retry

        /// <summary>
        /// رد کردن دستور تلاش مجدد - فقط توسط عضو مسئول (assignee)
        /// </summary>
        public Result RejectRetry(ActingMember assignee) {

            var authResult = EnsureIsAssignee(assignee);
            if (authResult.IsFailure) return authResult;

            return TransitionTo
                (TeamTaskStatus.RejectedToTryAgain,
                 TeamTaskStatus.OrderedToTryAgain, assignee);
        }

        #endregion

        #region Domain-Method: Close as Complete

        /// <summary>
        /// بستن نهایی وظیفه بعنوان موفق - فقط توسط عضو محول کننده یا لیدر
        /// </summary>
        public Result CloseAsComplete(ActingMember actor) {

            var authResult = EnsureIsManagerCreatorOrLeader(actor);
            if (authResult.IsFailure) return authResult;

            // فقط برای وظیفه ای که در وضعیت «اعلام موفقیت» یا «پذیرفته برای
            // تلاش مجدد» باشه میشه بعنوان موفق مختوم اعلام کرد
            if (Status is not (TeamTaskStatus
                .ClaimedAsComplete or TeamTaskStatus.AcceptedToTryAgain))
                return DomainErrors.TeamTask.InvalidTransition.AsResult();

            return SetStatus(TeamTaskStatus.ClosedAsComplete, actor);
        }

        #endregion

        #region Domain-Method: Close as Failure

        /// <summary>
        /// بستن نهایی وظیفه بعنوان شکست - فقط توسط عضو محول کننده یا لیدر
        /// </summary>
        public Result CloseAsFailure(ActingMember actor) {

            var authResult = EnsureIsManagerCreatorOrLeader(actor);
            if (authResult.IsFailure) return authResult;

            // فقط برای وظیفه ای که در وضعیت «اعلام شکست» یا «رد شده برای
            // تلاش مجدد» باشه میشه بعنوان شکست مختوم اعلام کرد
            if (Status is not (TeamTaskStatus
                .ClaimedAsFailure or TeamTaskStatus.RejectedToTryAgain))
                return DomainErrors.TeamTask.InvalidTransition.AsResult();

            return SetStatus(TeamTaskStatus.ClosedAsFailure, actor);
        }

        #endregion

        #region Domain-Method: Close as Deadline Failure

        /// <summary>
        /// بسته شدن خودکار وظیفه بابت گذشتن از ددلاین
        /// این متد توسط کاربر فراخوانی نمیشود، بلکه توسط یک فرآیند
        /// سیستمی در پس زمینه صدا زده میشود
        /// </summary>
        public Result CloseAsDeadlineFailure() {

            var alreadyClosed = Status is TeamTaskStatus.ClosedAsComplete
                or TeamTaskStatus.ClosedAsFailure or TeamTaskStatus.ClosedAsDeadlineFailure;

            if (alreadyClosed)
                return DomainErrors.TeamTask.InvalidTransition.AsResult();

            return SetStatus
                (TeamTaskStatus.ClosedAsDeadlineFailure,

                // اگر به پایان رسیدن مهلت ددلاین
                // منجر به تغییر وضعیت شود، از نظر سیستم
                // این تغییر توسط مسئول وظیفه انجام شده است
                new ActingMember
                        (AppointedToMemberId,
                         TeamMemberRole.Member));
        }

        #endregion

        // =======================================


        #region Internal-Shared Private Methods

        /// <summary>
        /// احراز دسترسی عضو مسئول
        /// </summary>
        private Result EnsureIsAssignee(ActingMember actor) {

            if (actor.MemberId != AppointedToMemberId)
                return DomainErrors.TeamTask.Unauthorized.AsResult();

            return Result.Success();
        }

        /// <summary>
        /// احراز دسترسی لیدر
        /// </summary>
        private Result EnsureIsLeader(ActingMember actor) {

            if (actor.Role != TeamMemberRole.Leader)
                return DomainErrors.TeamTask.Unauthorized.AsResult();

            return Result.Success();
        }

        /// <summary>
        /// احراز دسترسی کاربر محول کننده یا لیدر
        /// </summary>
        private Result EnsureIsCreatorOrLeader(ActingMember actor) {

            // در این بخش اختیارات کاربر محول کننده رو لیدر هم داره

            if (actor.MemberId != CreatedByMemberId &&
                actor.Role != TeamMemberRole.Leader)
                return DomainErrors.TeamTask.Unauthorized.AsResult();

            return Result.Success();
        }

        /// <summary>
        /// احراز دسترسی عضو محول کننده یا لیدر - با اجبار الزام مقام مدیر میانی برای غیر لیدر -
        /// شرایط لازم: عضو باید لیدر باشد - یا محول کننده ای باشد که مقام مدیر میانی دارد
        /// </summary>
        private Result EnsureIsManagerCreatorOrLeader(ActingMember actor) {

            // در این بخش اختیارات مدیر میانی محول کننده رو لیدر هم داره

            if (actor.MemberId != CreatedByMemberId
                && actor.Role != TeamMemberRole.Leader)
                return DomainErrors.TeamTask.Unauthorized.AsResult();

            // محول کننده ای که مقام مدیر میانی رو از دست داده هم مجاز نیست
            if (actor.Role is TeamMemberRole.Member)
                return DomainErrors.TeamTask.Unauthorized.AsResult();

            return Result.Success();
        }

        /// <summary>
        /// احراز دسترسی عضو محول کننده یا لیدر یا خویش مسئول -
        /// شرایط لازم: یا باید لیدر باشد - یا محول کننده ای باشد که مقام مدیر میانی دارد - 
        /// یا اگر مقام مدیر میانی ندارد، باید هم محول کننده و هم مسئول انجام دادن وظیفه باشد
        /// </summary>
        private Result EnsureIsManagerCreatorOrLeaderOrSelfTasker(ActingMember actor) {

            // در این بخش اختیارات مدیر میانی محول کننده رو لیدر هم داره

            if (actor.MemberId != CreatedByMemberId
                && actor.Role != TeamMemberRole.Leader)
                return DomainErrors.TeamTask.Unauthorized.AsResult();

            // محول کننده ای که مقام مدیر میانی ندارد و مسئول انجام وظیفه نیست هم مجاز نیست
            if (actor.Role is TeamMemberRole.Member
                && AppointedToMemberId != actor.MemberId)
                return DomainErrors.TeamTask.Unauthorized.AsResult();

            return Result.Success();
        }

        /// <summary>
        /// اعتبار سنجی و تغییر وضعیت وظیفه
        /// </summary>
        private Result TransitionTo(TeamTaskStatus target, TeamTaskStatus requiredCurrent, ActingMember actor) {

            if (Status != requiredCurrent)
                return DomainErrors.TeamTask.InvalidTransition.AsResult();

            return SetStatus(target, actor);
        }

        /// <summary>
        /// اعتبار سنجی و تغییر وضعیت وظیفه
        /// </summary>
        private Result TransitionTo(ActingMember actor,
            TeamTaskStatus target, params TeamTaskStatus[] requiredCurrents) {

            if (!requiredCurrents.Contains(Status))
                return DomainErrors.TeamTask.InvalidTransition.AsResult();

            return SetStatus(target, actor);
        }

        /// <summary>
        /// تغییر مستقیم وضعیت وظیفه
        /// </summary>
        private Result SetStatus(TeamTaskStatus status, ActingMember actor) {

            var oldStatus = Status;

            Status = status;
            LastModifiedAtUtc = DateTime.UtcNow;

            RaiseDomainEvent(new TeamTaskStatusChangedEvent
                (Id,
                 TeamId,
                 CreatedByMemberId,
                 AppointedToMemberId,
                 actor.MemberId,
                 oldStatus,
                 status,
                 Title.Value));

            return Result.Success();
        }

        #endregion
    }
}
