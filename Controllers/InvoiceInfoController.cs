using System.Text.Json;
using InvoiceHub.Dto;
using InvoiceHub.Services.InvoiceInformation;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceHub.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceInfoController(IInvoiceInforService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        // Convert object value thành JSON string
        var invoice = new Models.InvoiceInformation
        {
            Type = dto.Type,
            Value = JsonSerializer.Serialize(dto.Value),
            Key = dto.Key
        };
        
        await service.CreateAsync(invoice);
        return Ok(new { message = "Invoice created successfully" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateInvoiceDto dto)
    {
        var invoice = new Models.InvoiceInformation
        {
            Type = dto.Type,
            Value = JsonSerializer.Serialize(dto.Value)
        };
        
        await service.UpdateAsync(id, invoice);
        return Ok(new { message = "Invoice updated successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await service.GetAllAsync(page, pageSize);
        
        // Convert response sang DTO với Value là object
        var response = result.Select(x => new InvoiceResponseDto
        {
            Id = x.Id,
            Type = x.Type,
            EntityType = x.EntityType,
            Value = JsonSerializer.Deserialize<object>(x.Value)
        }).ToList();
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        
        // Convert response sang DTO với Value là object
        var response = new InvoiceResponseDto
        {
            Id = result.Id,
            Type = result.Type,
            EntityType = result.EntityType,
            Value = JsonSerializer.Deserialize<object>(result.Value)
        };
        
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
