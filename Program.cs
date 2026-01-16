using InvoiceHub;
using InvoiceHub.Interfaces;
using InvoiceHub.Services;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();