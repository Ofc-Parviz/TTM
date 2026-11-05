using MediatR;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Abstractions.Messaging
{

    /// <summary>
    /// قرارداد پیاده ساز های کامند لایه اپلیکیشن
    /// </summary>
    public interface ICommand : IRequest<Result>;

    /// <summary>
    /// قرارداد پیاده ساز های کامند لایه اپلیکیشن - بهمراه پاسخ
    /// </summary>
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
}
