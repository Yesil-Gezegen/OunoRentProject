using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.DTO;

namespace BusinessLayer.ActionFilters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = (IValidator)context.HttpContext.RequestServices.GetService(validatorType);
            if (validator == null) continue;

            var validationResult = validator.Validate(new ValidationContext<object>(argument));
            if (!validationResult.IsValid)
            {
                var errorResponse = new ErrorResponse
                {
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };

                foreach (var error in validationResult.Errors)
                {
                    if (!errorResponse.Errors.ContainsKey(error.PropertyName))
                    {
                        errorResponse.Errors[error.PropertyName] = new List<string>();
                    }
                    errorResponse.Errors[error.PropertyName].Add(error.ErrorMessage);
                }

                context.Result = new BadRequestObjectResult(errorResponse);
            }
        }
        base.OnActionExecuting(context);
    }
}