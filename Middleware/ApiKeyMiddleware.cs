using System.Net;

namespace InvoiceHub.Middleware;

public class ApiKeyMiddleware(RequestDelegate next)
{
    private const    string          API_KEY_HEADER = "X-API-KEY";


    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKey))
        {
            context.Items["ApiKey"] = apiKey.ToString();
        }

        await next(context);
    }
}