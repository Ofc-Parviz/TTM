using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Entities
{
    /// <summary>
    /// انتیتی دعوتنامه عضویت در تیم - چایلد انتیتی داخل مرز اگریگیت تیم
    /// </summary>
    public class TeamInvite : BaseEntity {

        #region Internal Constructor

        /// <summary>
        /// کانستراکتور داخلی
        /// </summary>
        private TeamInvite(Guid id,
            Guid hostTeamId,
            Guid senderMemberId,
            Guid receiverUserId,
            TeamMemberRole invitedRole,
            DisplayName invitedRoleTitle) : base(id) {

            HostTeamId = hostTeamId;
            SenderMemberId = senderMemberId;
            ReceiverUserId = receiverUserId;
            SuggestedAccessLevel = invitedRole;
            SuggestedRoleTitle = invitedRoleTitle;
            Status = TeamInviteStatus.Sent;
        }

        /// <summary>
        /// کانستراکتور برای ای-اف-کور تا بتونه مایگرشن بزنه
        /// </summary>
        #pragma warning disable CS8618
        private TeamInvite() : base(Guid.Empty) { }

        #endregion

        #region Domain Properties

        /// <summary>
        /// شناسه تیم میزبان
        /// </summary>
        public Guid HostTeamId { get; private set; }

        /// <summary>
        /// فرستنده دعوتنامه - شناسه عضو دعوت کننده
        /// </summary>
        public Guid SenderMemberId { get; private set; }

        /// <summary>
        /// سطح دسترسی کاربر دعوت شونده - در صورت پذیرش و عضویت
        /// </summary>
        public TeamMemberRole SuggestedAccessLevel { get; private set; }

        /// <summary>
        /// نام نقشی که قرار است کاربر دعوت شونده در تیم ایفا 
        /// کند - مثال: توسعه دهنده فرانت، گرافیست، توسعه دهنده بک-اند
        /// </summary>
        public DisplayName SuggestedRoleTitle { get; private set; }

        /// <summary>
        /// شناسه کاربر دعوت شونده
        /// </summary>
        public Guid ReceiverUserId { get; private set; }

        /// <summary>
        /// وضعیت دعوتنامه
        /// </summary>
        public TeamInviteStatus Status { get; private set; }

        #endregion

        #region Internal-Domain Methods

        internal static Result<TeamInvite> Create
            (Guid teamId, Guid senderMemberId, Guid receiverUserId,
            TeamMemberRole invitedAccessLevel, DisplayName invitedRoleTitle) {

            return new TeamInvite(Guid.NewGuid(),
                teamId, senderMemberId, receiverUserId,
                invitedAccessLevel, invitedRoleTitle);
        }

        internal void Accept() {
            Status = TeamInviteStatus.Accepted;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        internal void Reject() {
            Status = TeamInviteStatus.Rejected;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        internal void Cancel() {
            Status = TeamInviteStatus.Cancelled;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// لغو خودکار توسط سیستم - برای زمانی که کاربر دعوت شونده
        /// از طریق یک دعوتنامه دیگر از قبل عضو تیم شده باشد
        /// </summary>
        internal void AutoCancel() {

            Status = TeamInviteStatus.AutoCancelled;

            LastModifiedAtUtc = DateTime.UtcNow;

        }

        #endregion
    }
}
