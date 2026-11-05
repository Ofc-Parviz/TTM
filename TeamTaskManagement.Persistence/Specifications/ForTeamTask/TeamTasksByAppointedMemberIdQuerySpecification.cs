using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForTeamTask;

namespace TeamTaskManagement.Persistence.Specifications.ForTeamTask {
    /// <summary>
    /// اسپاسیفیکیشن برای ساخت کوئری دیتابیس - اینجا تعریف میشه که
    /// چجوری باید اطلاعات «وظایفی که به عضو تیم محول شده» رو از دیتابیس بخونیم
    /// </summary>
    public sealed class TeamTasksByAppointedMemberIdQuerySpecification : QuerySpecification<TeamTask> {
        public TeamTasksByAppointedMemberIdQuerySpecification(Guid appointedToMemberId)

            // قانون از دامین میاد، موارد لازم اینجا اضافه میشه
            : base(new TeamTasksByAppointedMemberIdSpecification(appointedToMemberId)) {

            AddOrderByDescending(t => t.CreatedAtUtc);
        }
    }
}