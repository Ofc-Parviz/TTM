using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Primitives;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Domain.Entities
{
    /// <summary>
    /// انتیتی عضو تیم - انتیتی فرزند داخل مرز اگریگیت تیم
    /// ساخته و مدیریت میشود فقط از طریق متدهای دامین تیم
    /// </summary>
    public class TeamMember : BaseEntity {

        #region Internal Constructor

        /// <summary>
        /// متد سازنده داخلی
        /// </summary>
        private TeamMember(Guid id,
            Guid teamId,
            Guid userId,
            TeamMemberStatus status,
            TeamMemberRole role,
            DisplayName roleDisplayname) : base(id) {

            TeamId = teamId;
            UserId = userId;
            Status = status;
            AccessLevel = role;
            RoleDisplayname = roleDisplayname;
            LastStatusChangeAt = DateTime.UtcNow;
        }

        /// <summary>
        /// کانستراکتور برای ای-اف-کور تا بتونه مایگرشن بزنه
        /// </summary>
        #pragma warning disable CS8618
        private TeamMember() : base(Guid.Empty) { }

        #endregion

        #region Domain Properties

        /// <summary>
        /// شناسه تیم مرتبط
        /// </summary>
        public Guid TeamId { get; private set; }

        /// <summary>
        /// شناسه کاربر عضو - ارجاع فقط با آیدی به اگریگیت کاربر
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// وضعیت فعلی عضو
        /// </summary>
        public TeamMemberStatus Status { get; private set; }

        /// <summary>
        /// سطح دسترسی - گزینه ها: لیدر - مدیرمیانی - عضو
        /// </summary>
        public TeamMemberRole AccessLevel { get; private set; }

        /// <summary>
        /// نام نقش ایفا شده در تیم - مثال: توسعه دهنده فرانت، دیزاینر
        /// </summary>
        public DisplayName RoleDisplayname { get; private set; }

        /// <summary>
        /// تاریخ آخرین باری که وضعیت عضویت تغییر کرد
        /// </summary>
        public DateTime LastStatusChangeAt { get; private set; }

        #endregion

        #region Internal-Domain Methods

        /// <summary>
        /// ایجاد عضو تیم - دسترسی فقط از اگریگیت تیم
        /// </summary>
        internal static Result<TeamMember> Create(
           Guid userId, Guid teamId, TeamMemberStatus status,
           TeamMemberRole role, DisplayName roleDisplayname) {

            if (userId == Guid.Empty) {
                return Result.Failure<TeamMember>(DomainErrors.TeamMember.UserIdEmpty);
            }

            return new TeamMember(Guid.NewGuid(), teamId, userId, status, role, roleDisplayname);
        }

        /// <summary>
        /// تغییر سطح دسترسی عضو - دسترسی فقط از اگریگیت تیم
        /// </summary>
        internal void ChangeRole(TeamMemberRole role, DisplayName roleDisplayname) {
            AccessLevel = role;
            RoleDisplayname = roleDisplayname;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// تغییر عنوان نقش عضو - دسترسی فقط از اگریگیت تیم
        /// </summary>
        internal void ChangeRoleTitle(DisplayName roleDisplayname) {
            RoleDisplayname = roleDisplayname;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// ترک کردن تیم توسط عضو - دسترسی فقط از اگریگیت تیم
        /// </summary>
        internal void Leave() {
            Status = TeamMemberStatus.Leaved;
            LastStatusChangeAt = DateTime.UtcNow;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// حذف عضو از تیم توسط لیدر - دسترسی ففط از اگریگیت تیم
        /// </summary>
        internal void Remove() {
            Status = TeamMemberStatus.Removed;
            LastStatusChangeAt = DateTime.UtcNow;
            LastModifiedAtUtc = DateTime.UtcNow;
        }

        #endregion

        #region Common Helper Methods

        /// <summary>
        /// تبدیل با عضو درخواست کننده عملیات
        /// </summary>
        public ActingMember AsActor() => new ActingMember(Id, AccessLevel);

        #endregion
    }
}
