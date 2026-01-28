using InvoiceHub;
using InvoiceHub.Data;
using InvoiceHub.Interfaces;
using InvoiceHub.Middleware;
using InvoiceHub.Services;
using InvoiceHub.Services.InvoiceInformation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddScoped<InvoiceMappingEngine>();
builder.Services.AddScoped<BkavService>();
builder.Services.AddScoped<VnptService>();
builder.Services.AddScoped<InvoiceFactory>();
builder.Services.AddScoped<IInvoiceInforService, InvoiceInforService>();
builder.Services.AddScoped<IApiKeyProvider, ApiKeyProvider>();
builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string" + "'DefaultConnect' not found");

builder.Services.AddDbContext<AppDbContext>(options =>
                                                options.UseSqlServer(connectionString));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiKeyMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();