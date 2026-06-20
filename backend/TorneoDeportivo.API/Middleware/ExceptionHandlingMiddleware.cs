using FluentValidation;
using TorneoDeportivo.Application.Common.Exceptions;

namespace TorneoDeportivo.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Uno o más campos no son válidos.",
                errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
    }
}
