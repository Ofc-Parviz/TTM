using MediatR;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Abstractions.Messaging
{
    /// <summary>
    /// قرارداد کوئری هندلر های لایه اپلیکیشن
    /// </summary>
    public interface IQueryHandler<TQuery,TResponse>
        : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
}
