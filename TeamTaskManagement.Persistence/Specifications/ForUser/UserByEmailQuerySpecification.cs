using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Specifications.ForUser;

namespace TeamTaskManagement.Persistence.Specifications.ForUser {
    public sealed class UserByEmailQuerySpecification : QuerySpecification<User> {
        public UserByEmailQuerySpecification(string email)
            : base(new UserByEmailSpecification(email)) {

        }
    }
}