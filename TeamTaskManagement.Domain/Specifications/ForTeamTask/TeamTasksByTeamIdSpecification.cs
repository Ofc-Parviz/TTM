using System.Linq.Expressions;
using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Specifications.ForTeamTask {
    /// <summary>
    /// دریافت اطلاعات وظیفه های مربوط به تیم
    /// </summary>
    public sealed class TeamTasksByTeamIdSpecification : Specification<TeamTask> {

        private readonly Guid _teamId;

        public TeamTasksByTeamIdSpecification(Guid teamId) => _teamId = teamId;

        public override Expression<Func<TeamTask, bool>> ToExpression()
            => task => task.TeamId == _teamId;
    }
}