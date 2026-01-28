using System.Text.Json;
using System.Xml;
using Newtonsoft.Json;
using Scriban;

namespace InvoiceHub;

public class InvoiceMappingEngine
{
    public string TransformToJson(string mappingJson, object input)
    {
        var template = Template.Parse(mappingJson);
        var json     = template.Render(input);
        try
        {
            JsonDocument.Parse(json);
            return json;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Invalid JSON output from mapping: {e.Message}", e);
        }
    }

 public string TransformToXml(string mappingJson, object input, string rootName)
    {
        if (string.IsNullOrWhiteSpace(rootName))
            throw new ArgumentException("XML root name is required", nameof(rootName));

        var template = Template.Parse(mappingJson);

        if (template.HasErrors)
            throw new InvalidOperationException(
                $"Scriban error: {string.Join("\n", template.Messages)}");

        var json = template.Render(input);

        try
        {
            // Step 1: Convert JSON to XML
            var xmlDoc = JsonConvert.DeserializeXmlNode(json, rootName);
            
            if (xmlDoc == null)
                throw new InvalidOperationException("Failed to convert JSON to XML");

            // Step 2: Add namespaces to root element
            var root = xmlDoc.DocumentElement;
            if (root != null)
            {
                root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                root.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            }

            // Step 3: Process null values to add xsi:nil="true"
            ProcessNullNodes(xmlDoc);

            // Step 4: Format output with declaration
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                OmitXmlDeclaration = false,
                Encoding = System.Text.Encoding.UTF8
            });
            
            xmlDoc.Save(xmlWriter);
            xmlWriter.Flush();
            
            return stringWriter.ToString();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error converting to XML: {e.Message}", e);
        }
    }

    private void ProcessNullNodes(XmlDocument xmlDoc)
    {
        // Find all empty nodes that should have xsi:nil="true"
        var emptyNodes = xmlDoc.SelectNodes("//*[not(node()) and not(@*)]");
        
        if (emptyNodes == null) return;

        var xsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";
        
        // foreach (XmlNode node in emptyNodes)
        // {
        //     if (node is XmlElement element)
        //     {
        //         // Check if this should be nil (based on your JSON mapping logic)
        //         // You can customize this logic based on your needs
        //         if (ShouldBeNil(element))
        //         {
        //             element.SetAttribute("nil", xsiNamespace, "true");
        //         }
        //     }
        // }
    }

    private bool ShouldBeNil(XmlElement element)
    {
        // Define which empty elements should have xsi:nil="true"
        // Based on VNPT requirements
        var nilElements = new[]
        {
            "ArisingDate",
            "ExchangeRate", 
            "ConvertedAmount",
            "MaCuaCQT",
            "UserDefine",
            "PassportNumber",
            "FiscalCodes",
            "OriginalInvoiceIdentify"
        };

        return nilElements.Contains(element.Name);
    }
}