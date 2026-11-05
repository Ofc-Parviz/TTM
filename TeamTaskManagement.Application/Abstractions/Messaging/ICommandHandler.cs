using MediatR;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Abstractions.Messaging
{

    /// <summary>
    /// قرارداد پیاده ساز های کامندهندلر در لایه اپلیکیشن
    /// </summary>
    public interface ICommandHandler<TCommand>
    : IRequestHandler<TCommand, Result> where TCommand: ICommand;

    /// <summary>
    /// قرارداد پیاده ساز های کامندهندلر در لایه اپلیکیشن - بهمراه پاسخ
    /// </summary>
    public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>> where TCommand: ICommand<TResponse>;
}
