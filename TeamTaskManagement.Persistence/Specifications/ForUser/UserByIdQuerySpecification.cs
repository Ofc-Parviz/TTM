using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForUser;

namespace TeamTaskManagement.Persistence.Specifications.ForUser {
    public sealed class UserByIdQuerySpecification : QuerySpecification<User> {
        public UserByIdQuerySpecification(Guid userId)
            : base(new UserByIdSpecification(userId)) {

        }
    }
}