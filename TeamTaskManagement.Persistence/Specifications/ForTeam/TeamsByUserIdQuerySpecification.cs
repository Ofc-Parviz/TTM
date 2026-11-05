using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForTeam;

namespace TeamTaskManagement.Persistence.Specifications.ForTeam {
    /// <summary>
    /// اسپاسیفیکیشن برای ساخت کوئری دیتابیس - اینجا تعریف میشه که
    /// چجوری باید اطلاعات «تیم هایی که کاربر توشون عضوه» رو از دیتابیس بخونیم
    /// </summary>
    public sealed class TeamsByUserIdQuerySpecification : QuerySpecification<Team> {
        public TeamsByUserIdQuerySpecification(Guid userId, bool onlyCurrentActiveTeams)

            // قانون از دامین میاد، موارد لازم اینجا اضافه میشه
            : base(new TeamsByUserIdSpecification(userId, onlyCurrentActiveTeams)) {

            AddInclude(team => team.MembersOfAllTimeHistory);
            AddInclude(team => team.Invites);
            IsSplitQuery = true;

            AddOrderByDescending(t => t.CreatedAtUtc);
        }
    }
}