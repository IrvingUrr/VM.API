using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Core.Exceptions;

namespace VM.Core.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {

        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilterAttribute()
        {
            // Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(InvalidCurrencyException), HandleInvalidCurrencyException },
                { typeof(BadRequestException), HandleBadRequestException },
                { typeof(ForbiddenException), HandleForbiddenException },
                { typeof(InternalServerErrorException), HandleInternalServerErrorException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(RequiredFieldException), HandleRequiredFieldException },
                { typeof(EntityNotFoundException), HandleEntityNotFoundException },
                { typeof(ConflictException), HandleConflictException },
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Log.Error(context.Exception, "Handling exception:");

            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            ProblemDetails details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            ValidationProblemDetails details = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleInvalidCurrencyException(ExceptionContext context)
        {
            InvalidCurrencyException? exception = context.Exception as InvalidCurrencyException;

            ProblemDetails details = new()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Currency iso code was not found.",
                Detail = exception!.Message
            };

            context.Result = new NotFoundObjectResult(details)
            {
                StatusCode = StatusCodes.Status404NotFound
            };

            context.ExceptionHandled = true;
        }

        private void HandleBadRequestException(ExceptionContext context)
        {
            BadRequestException? exception = context.Exception as BadRequestException;

            ProblemDetails details = new()
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Title = "Bad Request.",
                Detail = exception!.Message
            };

            context.Result = new BadRequestObjectResult(details)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            context.ExceptionHandled = true;
        }

        private void HandleForbiddenException(ExceptionContext context)
        {
            ForbiddenException? exception = context.Exception as ForbiddenException;

            ProblemDetails details = new()
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
                Title = "Forbidden.",
                Detail = exception!.Message
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            context.ExceptionHandled = true;
        }

        private void HandleInternalServerErrorException(ExceptionContext context)
        {
            InternalServerErrorException? exception = context.Exception as InternalServerErrorException;

            ProblemDetails details = new()
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error.",
                Detail = exception!.Message
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            NotFoundException? exception = context.Exception as NotFoundException;

            ProblemDetails details = new()
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found.",
                Detail = exception!.Message
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status404NotFound
            };

            context.ExceptionHandled = true;
        }

        private void HandleRequiredFieldException(ExceptionContext context)
        {
            RequiredFieldException? exception = context.Exception as RequiredFieldException;

            ProblemDetails details = new()
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Title = "Required fields are missing.",
                Detail = exception!.Message
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            context.ExceptionHandled = true;
        }

        private void HandleEntityNotFoundException(ExceptionContext context)
        {
            EntityNotFoundException exception = context.Exception as EntityNotFoundException;

            ProblemDetails details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception.Message
            };

            context.Result = new NotFoundObjectResult(details)
            {
                StatusCode = StatusCodes.Status404NotFound
            };

            context.ExceptionHandled = true;
        }

        private void HandleConflictException(ExceptionContext context)
        {
            ConflictException exception = context.Exception as ConflictException;

            ProblemDetails details = new ProblemDetails()
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                Title = "Conflict ocurred during processing.",
                Detail = exception.Message
            };

            context.Result = new NotFoundObjectResult(details)
            {
                StatusCode = StatusCodes.Status409Conflict
            };

            context.ExceptionHandled = true;
        }
    }
}
