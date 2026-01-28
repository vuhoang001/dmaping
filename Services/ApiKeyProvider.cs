using InvoiceHub.Interfaces;

namespace InvoiceHub.Services;

public class ApiKeyProvider(IHttpContextAccessor httpContextAccessor) : IApiKeyProvider
{
    public string? GetApiKey()
    {
        return httpContextAccessor.HttpContext?
            .Items["ApiKey"]?
            .ToString();
    }
}