using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForTeam;

namespace TeamTaskManagement.Persistence.Specifications.ForTeam {
    /// <summary>
    /// اسپاسیفیکیشن برای ساخت کوئری دیتابیس - اینجا تعریف میشه که
    /// چجوری باید اطلاعات «تیم» بر اساس «شناسه» از دیتابیس خونده بشه
    /// </summary>
    public sealed class TeamByIdQuerySpecification : QuerySpecification<Team> {
        public TeamByIdQuerySpecification(Guid teamId)

            // قانون از دامین میاد، موارد لازم اینجا اضافه میشه
            : base(new TeamByIdSpecification(teamId)) {

            AddInclude(team => team.MembersOfAllTimeHistory);
            AddInclude(team => team.Invites);

            // اگر حجم داده بالا باشد، SplitQuery بهتر است
            IsSplitQuery = true;
        }
    }
}