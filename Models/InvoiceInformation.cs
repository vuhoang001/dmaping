namespace InvoiceHub.Models;

public class InvoiceInformation
{
    public int Id { get; set; }

    public string Type { get; set; } = string.Empty;       // bkav, vnpt

    public string EntityType { get; set; } = string.Empty; // CLR type name

    public string Value { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}