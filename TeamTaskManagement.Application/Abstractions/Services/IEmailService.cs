using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Application.Enums;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Abstractions.Services
{
    /// <summary>
    /// قرارداد سرویس های ایمیل لایه اپلیکیشن
    /// </summary>
    public interface IEmailService {

        #region Team-Related Signetures

        /// <summary>
        /// ارسال ایمیل تشکر به
        /// مناسبت ساختن تیم
        /// </summary>
        /// <param name="receiverEmail">
        /// نشانی ایمیل دریافت کننده
        /// </param>
        /// <param name="receiverDisplayname">
        /// نام نمایشی دریافت کننده
        /// </param>
        /// <param name="createdTeamDisplayname">
        /// نام نمایشی تیم ساخته شده
        /// </param>
        Task<Result> SendTeamCreationEmailAsync
            (string receiverEmail,
             string receiverDisplayname,
             string createdTeamDisplayname, CancellationToken ct = default);

        /// <summary>
        /// ارسال ایمیل اطلاع رسانی به
        /// به کاربری که دعوت شده به تیم
        /// </summary>
        /// <param name="receiverEmail">
        /// نشانی ایمیل دریافت کننده
        /// </param>
        /// <param name="receiverDisplayname">
        /// نام نمایشی دریافت کننده
        /// </param>
        /// <param name="senderDisplayname">
        /// نام نمایشی فرستنده دعوتنامه
        /// </param>
        /// <param name="suggestedRoleTitle">
        /// نام نمایشی نقش پیشنهاد شده
        /// </param>
        /// <param name="hostTeamDisplayname">
        /// نام نمایشی تیم میزبان
        /// </param>
        Task<Result> SendEmailToInvitedUserAsync
            (string receiverEmail,
             string receiverDisplayname,
             string suggestedRoleTitle,
             string senderDisplayname,
             string hostTeamDisplayname,
             CancellationToken ct = default);

        /// <summary>
        /// ارسال ایمیل اعلان گروهی به اعضای تیم به یکی از مناسبت های: 
        /// ملحق شدن عضو جدید - حذف شدن یک عضو - ترک تیم توسط یک عضو
        /// </summary>
        /// <param name="memberEmails">
        /// ایمیل های اعضا
        /// </param>
        /// <param name="teamDisplayname">
        /// نام نمایشی تیم مرتبط
        /// </param>
        /// <param name="affectedMemberDisplayname">
        /// نام نمایشی عضو تحت تاثیر
        /// </param>
        /// <param name="affectedMemberRoleTitle">
        /// نام نقش محول شده به عضو تحت تاثیر
        /// </param>
        /// <param name="membershipUpdateType">
        /// نوع اطلاع رسانی - یکی از سه حالت ذکر شده
        /// </param>
        Task<Result> SendMembershipUpdateEmailToTeamMembersAsync
            (string[] memberEmails,
             string teamDisplayname,
             string affectedMemberDisplayname,
             string affectedMemberRoleTitle,
             MembershipUpdateEmailType membershipUpdateType,
             CancellationToken ct = default);

        #endregion

        #region Team-Task Created Event Handler Relateds

        /// <summary>
        /// ارسال ایمیل اطلاع رسانی به عضو مسئول انجام وظیفه جدید
        /// </summary>
        Task<Result> SendTaskAssignedToMemberEmailAsync(
            string receiverEmail,
            string receiverDisplayname,
            string taskTitle,
            string teamDisplayname,
            string creatorDisplayname,
            DateTime? deadlineUtc);

        /// <summary>
        /// ارسال ایمیل اطلاع رسانی به لیدر تیم در مورد ایجاد وظیفه جدید توسط شخص دیگر
        /// </summary>
        Task<Result> SendTaskCreatedNotificationToLeaderEmailAsync(
            string leaderEmail,
            string leaderDisplayname,
            string taskTitle,
            string teamDisplayname,
            string creatorDisplayname,
            string assigneeDisplayname,
            DateTime? deadlineUtc);

        #endregion

        #region Team-Task Assignee-Rotated Event Handler Relateds

        /// <summary>
        /// اطلاع رسانی به لیدر در مورد تغییر مسئول انجام وظیفه
        /// </summary>
        Task<Result> SendTaskAssigneeRotatedToLeaderEmailAsync(
            string leaderEmail,
            string leaderDisplayname,
            string taskTitle,
            string teamDisplayname,
            string oldAssigneeDisplayname,
            string newAssigneeDisplayname,
            string rotatedByDisplayname);

        /// <summary>
        /// اطلاع رسانی به مسئول قبلی که دیگر مسئول این وظیفه نیست
        /// </summary>
        Task<Result> SendTaskAssigneeRemovedEmailAsync(
            string receiverEmail,
            string receiverDisplayname,
            string taskTitle,
            string teamDisplayname,
            string newAssigneeDisplayname);

        /// <summary>
        /// اطلاع رسانی به مسئول جدید که وظیفه به او منتقل شده
        /// </summary>
        Task<Result> SendTaskAssigneeAssignedEmailAsync(
            string receiverEmail,
            string receiverDisplayname,
            string taskTitle,
            string teamDisplayname,
            string rotatedByDisplayname);

        #endregion

        #region Team-Task Creator-Rotated Event Handler Relateds

        /// <summary>
        /// اطلاع رسانی به مسئول انجام وظیفه در مورد تغییر مدیر وظیفه
        /// </summary>
        Task<Result> SendTaskCreatorChangedToAssigneeEmailAsync(
            string receiverEmail,
            string receiverDisplayname,
            string taskTitle,
            string teamDisplayname,
            string newCreatorDisplayname,
            bool newCreatorIsAssigneeByItself);

        /// <summary>
        /// اطلاع رسانی به مدیر قبلی وظیفه که اختیاراتش سلب شده
        /// </summary>
        Task<Result> SendTaskCreatorRemovedEmailAsync(
            string receiverEmail,
            string receiverDisplayname,
            string taskTitle,
            string teamDisplayname,
            string newCreatorDisplayname);

        /// <summary>
        /// اطلاع رسانی به مدیر جدید وظیفه که منصوب شده
        /// </summary>
        Task<Result> SendTaskCreatorAssignedEmailAsync(
            string receiverEmail,
            string receiverDisplayname,
            string taskTitle,
            string teamDisplayname,
            string rotatedByDisplayname);

        #endregion

        #region Team-Task Status-Changed Event Handler Relateds

        /// <summary>
        /// اطلاع رسانی تغییر وضعیت وظیفه به مسئول
        /// انجام، وقتی تغییر توسط مدیر یا لیدر انجام شده
        /// </summary>
        Task<Result> SendTaskStatusChangedToAssigneeEmailAsync(
            string receiverEmail,
            string receiverDisplayname,
            string taskTitle,
            string teamDisplayname,
            TeamTaskStatus newStatus,
            string changedByDisplayname);

        /// <summary>
        /// اطلاع رسانی تغییر وضعیت وظیفه به مدیر
        /// وظیفه، وقتی تغییر توسط مسئول انجام شده
        /// </summary>
        Task<Result> SendTaskStatusChangedToCreatorEmailAsync(
            string receiverEmail,
            string receiverDisplayname,
            string taskTitle,
            string teamDisplayname,
            TeamTaskStatus newStatus,
            string changedByDisplayname);

        /// <summary>
        /// اطلاع رسانی تغییر وضعیت وظیفه به لیدر تیم
        /// </summary>
        Task<Result> SendTaskStatusChangedToLeaderEmailAsync(
            string leaderEmail,
            string leaderDisplayname,
            string taskTitle,
            string teamDisplayname,
            TeamTaskStatus newStatus,
            string changedByDisplayname,
            string assigneeDisplayname);

        #endregion

        #region Team-Task Deadline-Changed Event Handler Relateds

        /// <summary>
        /// اطلاع رسانی تغییر ددلاین وظیفه به مسئول انجام یا لیدر
        /// </summary>
        Task<Result> SendDeadlineChangeEmailAsync
            (string receiverEmail,
             string receiverDisplayname,
             string taskTitle,
             string teamDisplayname,
             string changedByDisplayname,
             DateTime? newDeadlineUtc,
             CancellationToken ct = default);

        #endregion

        #region Team-Task Details-Updated Event Handler Relateds

        /// <summary>
        /// اطلاع رسانی تغییر جزییات وظیفه به مسئول انجام یا لیدر
        /// </summary>
        Task<Result> SendDetailsUpdatedEmailAsync
            (string receiverEmail,
             string receiverDisplayname,
             string teamDisplayname,
             string changedByDisplayname,
             string newTeamTaskTitle,
             string newTeamTaskDescription,
             CancellationToken ct = default);

        #endregion



        /// <summary>
        /// پیغام خوش آمد گویی به کاربر جدیدی که ثبت نام کرده
        /// </summary>
        Task<Result> SendUserRegistrationWelcomeEmailAsync
            (string email,
             string userDisplayname,
             string userFirstname,
             string userLastname,
             CancellationToken ct = default);






    }
}
