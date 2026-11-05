using System.Linq.Expressions;
using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Specifications.ForTeam {
    /// <summary>
    /// دریافت اطلاعات تیم با شناسه عضو آن تیم
    /// </summary>
    public sealed class TeamByMemberIdSpecification : Specification<Team> {

        private readonly Guid _memberId;

        public TeamByMemberIdSpecification(Guid memberId) => _memberId = memberId;

        public override Expression<Func<Team, bool>> ToExpression()
            => team => team.MembersOfAllTimeHistory
                .Any(m => m.Id == _memberId);
    }
}