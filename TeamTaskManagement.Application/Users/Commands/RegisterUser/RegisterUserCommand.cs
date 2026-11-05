using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Users.Commands.RegisterUser {
    /// <summary>
    /// دستور ثبت نام کاربر جدید
    /// </summary>
    public sealed record RegisterUserCommand(
        string Email,
        string Password,
        string DisplayName,
        string FirstName,
        string LastName) : ICommand<Guid>;
}