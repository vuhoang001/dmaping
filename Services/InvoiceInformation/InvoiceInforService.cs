using InvoiceHub.Data;
using InvoiceHub.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceHub.Services.InvoiceInformation;

public class InvoiceInforService(AppDbContext context) : IInvoiceInforService
{
    public async Task CreateAsync(Models.InvoiceInformation invoice)
    {
        var entityType = GetClrType(invoice.Type);
        var isExisted =
            await context.InvoiceInfor.FirstOrDefaultAsync(x => x.Type == invoice.Type && x.Key == invoice.Key);
        if (isExisted is not null) return;

        context.InvoiceInfor.Add(new Models.InvoiceInformation
        {
            Type       = invoice.Type,
            EntityType = entityType.AssemblyQualifiedName!,
            Value      = invoice.Value, // Đã là JSON string từ Controller
            Key        = invoice.Key
        });

        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Models.InvoiceInformation invoice)
    {
        var result = await context.InvoiceInfor.FirstOrDefaultAsync(x => x.Id == id);
        if (result is null)
            throw new KeyNotFoundException($"Invoice {id} not found");

        var entityType = GetClrType(invoice.Type);
        result.Type       = invoice.Type;
        result.EntityType = entityType.AssemblyQualifiedName!;
        result.Value      = invoice.Value; // Đã là JSON string từ Controller

        await context.SaveChangesAsync();
    }

    public async Task<List<Models.InvoiceInformation>> GetAllAsync(int page, int pageSize)
    {
        var results = await context.InvoiceInfor
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return results; // Trả về JSON string, Controller sẽ deserialize
    }

    public async Task<Models.InvoiceInformation> GetByIdAsync(int id)
    {
        var result = await context.InvoiceInfor.FirstOrDefaultAsync(x => x.Id == id);
        if (result is null)
            throw new KeyNotFoundException($"Invoice {id} not found");

        return result; // Trả về JSON string, Controller sẽ deserialize
    }

    public async Task<Models.InvoiceInformation> GetByApiKeyAsync(string apiKey, string provider)
    {
        var result = await context.InvoiceInfor.FirstOrDefaultAsync(x => x.Key == apiKey && x.Type == provider);
        return result ?? throw new KeyNotFoundException();
    }

    public async Task DeleteAsync(int id)
    {
        var result = await context.InvoiceInfor.FirstOrDefaultAsync(x => x.Id == id);
        if (result is null)
            throw new KeyNotFoundException($"Invoice {id} not found");

        context.InvoiceInfor.Remove(result);
        await context.SaveChangesAsync();
    }

    private static Type GetClrType(string type) => type.ToLower() switch
    {
        "bkav" => typeof(BkavInfor),
        "vnpt" => typeof(VnptInfor),
        _      => throw new NotSupportedException($"Type '{type}' is not supported")
    };
}