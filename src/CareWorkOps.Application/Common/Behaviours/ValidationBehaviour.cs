using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Common.Behaviours
{
    public sealed class ValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(validator =>
                validator.ValidateAsync(context, cancellationToken)));

        var validationErrors = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .Select(error => error.ErrorMessage)
            .Distinct()
            .ToArray();

        if (validationErrors.Length == 0)
        {
            return await next();
        }

        var errorMessage = string.Join("; ", validationErrors);

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var responseType = typeof(TResponse).GetGenericArguments()[0];

            var failureMethod = typeof(Result<>)
                .MakeGenericType(responseType)
                .GetMethod(nameof(Result<object>.Failure), [typeof(Error)]);

            return (TResponse)failureMethod!.Invoke(
                null,
                [Error.Validation(errorMessage)])!;
        }

        throw new ValidationException(errorMessage);
    }
}
}
