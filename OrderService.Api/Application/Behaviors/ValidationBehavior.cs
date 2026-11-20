using FluentValidation;
using MediatR;
using System.Collections.Concurrent;
using OrderService.Api.Domain.Common;
using System.Linq.Expressions;

namespace OrderService.Api.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly ConcurrentDictionary<Type, Func<IReadOnlyList<Error>, object>?>
        FailureFactoryCache = new();

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var groupedErrors = failures
            .GroupBy(f => string.IsNullOrWhiteSpace(f.PropertyName) ? "General" : f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

        Error validationError = new ValidationError(
            Code: "Validation.Failed",
            Message: "Una o más validaciones de entrada fallaron.",
            FieldErrors: groupedErrors);

        IReadOnlyList<Error> errorList = [validationError];

        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(errorList);

        if (TryCreateGenericFailure<TResponse>(errorList, out var failureResult))
            return failureResult!;

        throw new ValidationException(failures);
    }

    private static bool TryCreateGenericFailure<T>(IReadOnlyList<Error> errors, out T? result)
    {
        var factory = FailureFactoryCache.GetOrAdd(typeof(T), BuildFactory);
        if (factory is null)
        {
            result = default;
            return false;
        }

        result = (T)factory(errors);
        return true;
    }

    private static Func<IReadOnlyList<Error>, object>? BuildFactory(Type responseType)
    {
        if (!responseType.IsGenericType ||
            responseType.GetGenericTypeDefinition() != typeof(Result<>))
        {
            return null;
        }

        var method = responseType.GetMethod(
            nameof(Result<object>.Failure),
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
            [typeof(IReadOnlyList<Error>)]);

        if (method is null)
            return null;


        var param = Expression.Parameter(typeof(IReadOnlyList<Error>), "errors");
        var call = Expression.Call(method, param);
        var convert = Expression.Convert(call, typeof(object));

        return Expression.Lambda<Func<IReadOnlyList<Error>, object>>(convert, param).Compile();
    }
}
