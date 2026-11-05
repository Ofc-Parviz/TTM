using System.Linq.Expressions;
using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Specifications.ForTeam {
    /// <summary>
    /// دریافت اطلاعات تیم با شناسه
    /// </summary>
    public sealed class TeamByIdSpecification : Specification<Team> {

        private readonly Guid _teamId;

        public TeamByIdSpecification(Guid teamId) => _teamId = teamId;

        public override Expression<Func<Team, bool>> ToExpression()
            => team => team.Id == _teamId;
    }
}
