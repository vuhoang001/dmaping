namespace InvoiceHub.Services.InvoiceInformation;

public interface IInvoiceInforService
{
    Task                                  CreateAsync(Models.InvoiceInformation invoice);
    Task                                  UpdateAsync(int id, Models.InvoiceInformation invoice);
    Task<List<Models.InvoiceInformation>> GetAllAsync(int page, int pageSize);
    Task<Models.InvoiceInformation>       GetByIdAsync(int id);
    Task<Models.InvoiceInformation>       GetByApiKeyAsync(string apiKey, string provider);
    Task                                  DeleteAsync(int id);
}