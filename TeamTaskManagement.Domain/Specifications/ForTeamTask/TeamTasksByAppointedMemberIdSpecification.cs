using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Specifications.ForTeamTask {
    /// <summary>
    /// دریافت اطلاعات وظایف با شناسه عضو مسئول
    /// </summary>
    public sealed class TeamTasksByAppointedMemberIdSpecification : Specification<TeamTask> {

        private readonly Guid _appointedToMemberId;

        public TeamTasksByAppointedMemberIdSpecification(Guid appointedToMemberId) => _appointedToMemberId = appointedToMemberId;

        public override Expression<Func<TeamTask, bool>> ToExpression()
            => task => task.AppointedToMemberId == _appointedToMemberId;
    }
}
