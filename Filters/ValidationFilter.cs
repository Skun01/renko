using System;
using FluentValidation;
using project_z_backend.Share;

namespace project_z_backend.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public readonly IValidator<T> _validator;
    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? argumentToValidate = context.Arguments.OfType<T>().FirstOrDefault();
        if (argumentToValidate is null)
            return await next(context);

        var validatorResult = await _validator.ValidateAsync(argumentToValidate);
        // if has validation error
        if (!validatorResult.IsValid)
        {
            var errors = validatorResult.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = ApiResponse.ErrorResponse(
                "Validation Failed",
                errors,
                400
            );

            return Results.BadRequest(response);
        }

        return await next(context);
    }
}
