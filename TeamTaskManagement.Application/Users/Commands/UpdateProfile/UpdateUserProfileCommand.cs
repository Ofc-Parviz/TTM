using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Users.Commands.UpdateProfile {
    /// <summary>
    /// دستور ویرایش پروفایل کاربر
    /// </summary>
    public sealed record UpdateUserProfileCommand(
        string DisplayName,
        string FirstName,
        string LastName) : ICommand;
}