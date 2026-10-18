using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandlers
{
    public class DeleteRestrictedExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            if (exception is not DeleteRestrictedException ex)
                return false;

            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Delete restricted",
                Detail = ex.Message
            }, ct);

            return true;
        }
    }
}
