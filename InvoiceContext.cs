namespace InvoiceHub;

public class InvoiceContext
{
    public string Provider { get; init; } = string.Empty;
    public Dictionary<string, object> Data { get; init; } = new();
}