using InvoiceHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController(InvoiceFactory invoiceFactory) : ControllerBase
{
    [HttpPost("create-invoice")]
    public async Task<IActionResult> CreateInvoice(InvoiceContext context)
    {
        var service = invoiceFactory.GetInvoiceService(context.Provider);
        var result  = await service.CreateInvoiceAsync(context);
        return Ok(result);
    }

    [HttpPost("replace-invoice")]
    public async Task<IActionResult> ReplaceInvoice(InvoiceContext context)
    {
        var servive = invoiceFactory.GetInvoiceService(context.Provider);
        var result  = await servive.ReplaceInvoiceAsync(context);
        return Ok(result);
    }

    [HttpPost("adjust-invoice")]
    public async Task<IActionResult> AdjustInvoice(InvoiceContext context)
    {
        var service = invoiceFactory.GetInvoiceService(context.Provider);
        var result  = await service.AdjustInvoiceAsync(context);
        return Ok(result);
    }
}