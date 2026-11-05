using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForTeamTask;

namespace TeamTaskManagement.Persistence.Specifications.ForTeamTask {
    /// <summary>
    /// اسپاسیفیکیشن برای ساخت کوئری دیتابیس - اینجا تعریف میشه که
    /// چجوری باید اطلاعات «وظایف باز» برای «عضو تیم» از دیتابیس خونده بشه
    /// </summary>
    public sealed class OpenTasksForMemberQuerySpecification : QuerySpecification<TeamTask> {
        public OpenTasksForMemberQuerySpecification(Guid memberId)

            // قانون از دامین میاد، موارد لازم اینجا اضافه میشه
            : base(new OpenTasksForMemberSpecification(memberId)) {

            AddOrderBy(task => task.CreatedAtUtc);
        }
    }
}
