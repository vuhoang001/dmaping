namespace InvoiceHub.Dto;

public class InvoiceResponse
{
   public object? XValue { get; set; } = string.Empty;
   public bool IsSuccess { get; set; }
   public string? Message { get; set; }
}


public class CreateInvoiceDto
{
   public string Type { get; set; } = string.Empty;
   public object Value { get; set; } = null!; // Accept object từ JSON
   public string Key { get; set; } = string.Empty;
}

// DTO cho response
public class InvoiceResponseDto
{
   public int Id { get; set; }
   public string Type { get; set; } = string.Empty;
   public string EntityType { get; set; } = string.Empty;
   public object? Value { get; set; } // Trả về object
}