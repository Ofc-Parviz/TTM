using MediatR;
using FluentValidation;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Behaviors
{
    public class ValidationPipelineBehavior
        <TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse> where TResponse : Result {

        #region Validators Injection

        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
            => _validators = validators;

        #endregion


        public async Task<TResponse> Handle
            (TRequest request,
             RequestHandlerDelegate<TResponse> next,
             CancellationToken ct) {

            if (!_validators.Any()) return await next();

            var errors = _validators
                .Select(validator => validator.Validate(request))
                .SelectMany(vResult => vResult.Errors)
                .Where(vFailure => vFailure is not null)
                .Select(failure => new Error(
                    failure.PropertyName,
                    failure.ErrorMessage))
                .Distinct()
                .ToArray();

            if (errors.Any()) return CreateValidationResult<TResponse>(errors);

            return await next(ct);
        }

        private static TResult CreateValidationResult
            <TResult>(Error[] errors) where TResult: Result {

            if (typeof(TResult) == typeof(Result))
                return (ValidationResult.WithErrors(errors) as TResult)!;

            object vResult = typeof(ValidationResult<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
                .GetMethod(nameof(ValidationResult.WithErrors))!
                .Invoke(null, new object?[] { errors })!;

            return (TResult)vResult;
        }
    }
}
