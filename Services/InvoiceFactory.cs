using InvoiceHub.Interfaces;

namespace InvoiceHub.Services;

public class InvoiceFactory(IServiceProvider serviceProvider)
{
    
    public IInvoiceService GetInvoiceService(string type)
    {
        return type.ToLower() switch
        {
            "vnpt" => serviceProvider.GetRequiredService<VnptService>(),
            "bkav" => serviceProvider.GetRequiredService<BkavService>(),
            _      => throw new NotSupportedException($"Provider '{type}' is not supported")
        };
    }
}