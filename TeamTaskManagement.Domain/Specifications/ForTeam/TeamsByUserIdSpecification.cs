using System.Linq.Expressions;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Specifications.ForTeam {
    /// <summary>
    /// تیم هایی که کاربر در آن ها عضویت فعال یا سابق دارد
    /// </summary>
    public sealed class TeamsByUserIdSpecification : Specification<Team> {

        private readonly Guid _userId;

        /// <summary>
        /// اگر ترو باشد، فقط تیم هایی که کاربر عضو آن هست بازخواهدگشت
        /// </summary>
        private readonly bool _onlyCurrentActiveTeams;

        public TeamsByUserIdSpecification(Guid userId, bool onlyCurrentActiveTeams) {
            _userId = userId;
            _onlyCurrentActiveTeams = onlyCurrentActiveTeams;
        }

        public override Expression<Func<Team, bool>> ToExpression()
            // اگر فقط تیم های عضو فعال خواسته شده بود
            => _onlyCurrentActiveTeams
            // وضعیت عضویت فعال رو هم فیلتر میکنیم
            // از فیلد کارنت ممبر چون توی ای اف کور مپ نشده استفاده نمیکنیم
            ? team => team.MembersOfAllTimeHistory.Any(m => m.UserId == _userId && m.Status == TeamMemberStatus.Active)
            // در غیر اینصورت، هر تیمی که کاربر سابقه عضویت داخلشو داشته برمیداریم
            : team => team.MembersOfAllTimeHistory.Any(m => m.UserId == _userId);
    }
}