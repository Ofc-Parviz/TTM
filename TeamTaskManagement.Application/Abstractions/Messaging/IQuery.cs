using MediatR;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Abstractions.Messaging
{
    /// <summary>
    /// قرارداد کوئری های لایه اپلیکیشن
    /// </summary>
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
}
