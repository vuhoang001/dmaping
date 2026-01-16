using InvoiceHub.Dto;

namespace InvoiceHub.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponse> CreateInvoiceAsync(InvoiceContext payload);
    Task<InvoiceResponse> AdjustInvoiceAsync(InvoiceContext payload);
    Task<InvoiceResponse> ReplaceInvoiceAsync(InvoiceContext payload);
}