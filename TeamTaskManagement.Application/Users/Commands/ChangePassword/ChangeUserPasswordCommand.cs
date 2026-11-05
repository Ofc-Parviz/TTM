using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Users.Commands.ChangePassword {
    /// <summary>
    /// دستور تغییر رمز عبور کاربر
    /// </summary>
    public sealed record ChangeUserPasswordCommand(
        string CurrentPassword,
        string NewPassword) : ICommand;
}