using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForTeamTask;

namespace TeamTaskManagement.Persistence.Specifications.ForTeamTask {
    /// <summary>
    /// اسپاسیفیکیشن برای ساخت کوئری دیتابیس - اینجا تعریف میشه که
    /// چجوری باید اطلاعات «کل وظایف یک تیم» رو از دیتابیس بخونیم
    /// </summary>
    public sealed class TeamTasksByTeamIdQuerySpecification : QuerySpecification<TeamTask> {
        public TeamTasksByTeamIdQuerySpecification(Guid teamId)

            // قانون از دامین میاد، موارد لازم اینجا اضافه میشه
            : base(new TeamTasksByTeamIdSpecification(teamId)) {

            AddOrderByDescending(t => t.CreatedAtUtc);
        }
    }
}