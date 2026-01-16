using InvoiceHub.BusinessService;
using InvoiceHub.Dto;
using InvoiceHub.Interfaces;
using InvoiceHub.PublishService;
using Newtonsoft.Json;

namespace InvoiceHub.Services;

public class VnptService(ILogger<VnptService> logger, IConfiguration configuration, InvoiceMappingEngine engine)
    : IInvoiceService
{
    private readonly string _account = configuration["Vnpt:Account"] ??
        throw new ArgumentException("Account not found in configuration");

    private readonly string _acPass = configuration["Vnpt:AcPass"] ??
        throw new ArgumentException("Account not found in configuration");

    private readonly string _username = configuration["Vnpt:Username"] ??
        throw new ArgumentException("Account not found in configuration");

    private readonly string _password = configuration["Vnpt:Password"] ??
        throw new ArgumentException("Account not found in configuration");

    private readonly string _pattern = configuration["Vnpt:Pattern"] ??
        throw new ArgumentException("Account not found in configuration");

    private readonly string _serial = configuration["Vnpt:Serial"] ??
        throw new ArgumentException("Account not found in configuration");

    private async Task<string> TransferToPayload(string? url = null)
    {
        var mappingPath = url ?? "Mappings/vnpt.mapping.json.scriban";

        if (!File.Exists(mappingPath))
            throw new FileNotFoundException($"Mapping: {mappingPath}");

        var mapping = await File.ReadAllTextAsync(mappingPath);


        return mapping;
    }

    public async Task<InvoiceResponse> CreateInvoiceAsync(InvoiceContext payload)
    {
        var client = new PublishServiceSoapClient(PublishServiceSoapClient.EndpointConfiguration.PublishServiceSoap);
        try
        {
            var xml              = await TransferToPayload();
            var plainCommandData = engine.TransformToXml(xml, payload.Data, "Invoices");


            var result = await client.ImportInvByPatternAsync(_account, _acPass, plainCommandData,
                                                              _username, _password, _pattern,
                                                              _serial, 0);

            logger.LogInformation(
                "Result of create invoice: {Result}",
                JsonConvert.SerializeObject(result)
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new InvoiceResponse();
    }

    public async Task<InvoiceResponse> AdjustInvoiceAsync(InvoiceContext payload)
    {
        var clientBusiness =
            new BusinessServiceSoapClient(BusinessServiceSoapClient.EndpointConfiguration.BusinessServiceSoap);


        var xml              = await TransferToPayload("Mappings/vnpt.adjust.mapping.json.scriban");
        var plainCommandData = engine.TransformToXml(xml, payload.Data, "AdjustInv");

        try
        {

            var result = await clientBusiness.AdjustInvoiceActionAsync(_account, _acPass,
                                                                       plainCommandData,
                                                                       _username, _password,
                                                                       payload.RefKey, "",
                                                                       0, _pattern, _serial);

            logger.LogInformation(
                "Result of replace invoice: {Result}",
                JsonConvert.SerializeObject(result)
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new InvoiceResponse();
    }

    public async Task<InvoiceResponse> ReplaceInvoiceAsync(InvoiceContext payload)
    {
        var clientBusiness =
            new BusinessServiceSoapClient(BusinessServiceSoapClient.EndpointConfiguration.BusinessServiceSoap);


        var xml              = await TransferToPayload("Mappings/vnpt.replace.mapping.json.scriban");
        var plainCommandData = engine.TransformToXml(xml, payload.Data, "ReplaceInv");

        try
        {

            var result = await clientBusiness.ReplaceActionAssignedNoAsync(_account, _acPass,
                                                                           plainCommandData,
                                                                           _username, _password,
                                                                           payload.RefKey, "",
                                                                           0, _pattern, _serial);

            logger.LogInformation(
                "Result of replace invoice: {Result}",
                JsonConvert.SerializeObject(result)
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new InvoiceResponse();
    }
}