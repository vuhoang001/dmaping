using Microsoft.AspNetCore.Mvc;

namespace InvoiceHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateInv([FromBody] InvoiceContext payload)
    {
        var mapping = await System.IO.File.ReadAllTextAsync($"Mappings/{payload.Provider}.mapping.json");
        var template = await  System.IO.File.ReadAllTextAsync($"Templates/{payload.Provider}.template.xml");

        var renderer = new InvoiceTemplateRenderer();
        var xml = renderer.Render(mapping, template, payload.Data);
        return Ok(xml);
    }
}