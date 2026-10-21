using ClaimManagement.Application.Wrapper;

namespace ClaimManagement.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        RequestDelegate next;
        ILogger<GlobalExceptionMiddleware> logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // the full error goes to the log, the client only sees a safe message
                logger.LogError(ex, "Unhandled error on {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                context.Response.StatusCode = 500;

                var response = ApiResponse<object>.Fail(
                    "Something went wrong. Please try again later.");

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
