using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandlers
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            logger.LogError(exception, "Unhandled exception");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 500,
                Title = "An unexpected error occurred."
            }, ct);
            return true;
        }
    }
}
