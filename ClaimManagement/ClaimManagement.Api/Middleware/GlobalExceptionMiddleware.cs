using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace ClaimManagement.Api.Middleware;

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
        catch (BadRequestException ex)
        {
            await Write(context, 400, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await Write(context, 404, ex.Message);
        }
        catch (ConflictException ex)
        {
            await Write(context, 409, ex.Message);
        }
        catch (ServiceUnavailableException ex)
        {
            logger.LogError(ex, "Dependent service problem on {Method} {Path}", context.Request.Method, context.Request.Path);
            await Write(context, 503, ex.Message);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // two users changed the same claim at the same time
            logger.LogWarning(ex, "Concurrency conflict on {Method} {Path}", context.Request.Method, context.Request.Path);
            await Write(context, 409, "This claim was changed by someone else. Please reload and try again.");
        }
        catch (Exception ex)
        {
            // the full error goes to the log, the client only sees a safe message
            logger.LogError(ex, "Unhandled error on {Method} {Path}", context.Request.Method, context.Request.Path);
            await Write(context, 500, "Something went wrong. Please try again later.");
        }
    }

    private static async Task Write(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(message));
    }
}