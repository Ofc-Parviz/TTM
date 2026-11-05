using System.Linq.Expressions;
using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Specifications.ForTeamTask {
    /// <summary>
    /// دریافت اطلاعات وظیفه با شناسه
    /// </summary>
    public sealed class TeamTaskByIdSpecification : Specification<TeamTask> {

        private readonly Guid _taskId;

        public TeamTaskByIdSpecification(Guid taskId) => _taskId = taskId;

        public override Expression<Func<TeamTask, bool>> ToExpression()
            => task => task.Id == _taskId;
    }
}