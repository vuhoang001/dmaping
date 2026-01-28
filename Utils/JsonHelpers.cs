using System.Text.Json;

namespace InvoiceHub.Utils;

public static class JsonHelpers
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string Serialize<T>(T data)
        => JsonSerializer.Serialize(data, Options);

    public static T? Deserialize<T>(string json)
        => JsonSerializer.Deserialize<T>(json, Options);

    public static object? Deserialize(string json, Type type)
        => JsonSerializer.Deserialize(json, type, Options);
}