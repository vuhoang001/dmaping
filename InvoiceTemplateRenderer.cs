using System.Text.Json;
using System.Text.Json.Nodes;

namespace InvoiceHub;

public class InvoiceTemplateRenderer
{
    public string Render(
        string mappingJson,
        string templateXml,
        object input)
    {
        var mappingTemplate = Scriban.Template.Parse(mappingJson);
        var mappedJson      = mappingTemplate.Render(input);

        var mappedNode = JsonNode.Parse(mappedJson);

        var xmlTemplate = Scriban.Template.Parse(templateXml);
        return xmlTemplate.Render(mappedNode, m => m.Name);
    }
}
