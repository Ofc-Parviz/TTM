using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForTeamTask;

namespace TeamTaskManagement.Persistence.Specifications.ForTeamTask {
    /// <summary>
    /// اسپاسیفیکیشن برای ساخت کوئری دیتابیس - اینجا تعریف میشه که
    /// چجوری باید اطلاعات «وظیفه» بر اساس «شناسه» رو از دیتابیس بخونیم
    /// </summary>
    public sealed class TeamTaskByIdQuerySpecification : QuerySpecification<TeamTask> {
        public TeamTaskByIdQuerySpecification(Guid taskId)

            // قانون از دامین میاد، موارد لازم اینجا اضافه میشه
            : base(new TeamTaskByIdSpecification(taskId)) {

            // فعلاً نیازی به Include نداره
        }
    }
}