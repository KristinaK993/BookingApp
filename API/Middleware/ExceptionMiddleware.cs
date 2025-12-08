using System.Net;
using System.Text.Json;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // skicka vidare requesten
        }
        catch (Exception ex)
        {
            // logga felet
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            // bygg ett standardiserat fel-svar
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var problem = new
            {
                status = context.Response.StatusCode,
                title = "Ett oväntat fel inträffade.",
                detail = ex.Message,   // i skarp miljö kan man ta bort detta
                traceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }
}
