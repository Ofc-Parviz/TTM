using System.Linq.Expressions;
using TeamTaskManagement.Domain.Entities;

namespace TeamTaskManagement.Domain.Specifications.ForUser {
    /// <summary>
    /// دریافت اطلاعات کاربر بر اساس شناسه
    /// </summary>
    public sealed class UserByIdSpecification : Specification<User> {

        private readonly Guid _userId;

        public UserByIdSpecification(Guid id) => _userId = id;

        public override Expression<Func<User, bool>> ToExpression()
            => user => user.Id == _userId;
    }
}