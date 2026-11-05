using TeamTaskManagement.Application.Users.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Users.Commands.LoginUser;

/// <summary>
/// دستور ورود کاربر
/// </summary>
public sealed record LoginUserCommand
    (string Email,
     string Password) : ICommand<LoginUserResponse>;
