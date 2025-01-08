using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Campus_Events.Misc
{
    public class GlobalExceptionHandler :  IExceptionHandler
    {
        private ILogger<GlobalExceptionHandler> logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this.logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "An error occured!");

            var problem = new ProblemDetails()
            {
                Title = exception.Message,
                Detail = exception.ToString()
            };
            var json = JsonSerializer.Serialize(problem);
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsync(json, cancellationToken);
            return true;
        }
    }
}
