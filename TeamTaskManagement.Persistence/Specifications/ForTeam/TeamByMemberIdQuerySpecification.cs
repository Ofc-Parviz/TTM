using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForTeam;

namespace TeamTaskManagement.Persistence.Specifications.ForTeam {
    /// <summary>
    /// اسپاسیفیکیشن برای ساخت کوئری دیتابیس - اینجا تعریف میشه که
    /// چجوری باید اطلاعات «تیم میزبان» برای «عضو تیم» رو از دیتابیس بخونیم
    /// </summary>
    public sealed class TeamByMemberIdQuerySpecification : QuerySpecification<Team> {
        public TeamByMemberIdQuerySpecification(Guid memberId)

            // قانون از دامین میاد، موارد لازم اینجا اضافه میشه
            : base(new TeamByMemberIdSpecification(memberId)) {

            AddInclude(team => team.MembersOfAllTimeHistory);
            AddInclude(team => team.Invites);
            IsSplitQuery = true;
        }
    }
}