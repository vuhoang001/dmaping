using InvoiceHub.BusinessService;
using InvoiceHub.Dto;
using InvoiceHub.Interfaces;
using InvoiceHub.Models;
using InvoiceHub.PublishService;
using InvoiceHub.Services.InvoiceInformation;
using InvoiceHub.Utils;
using Newtonsoft.Json;

namespace InvoiceHub.Services;

public class VnptService(
    ILogger<VnptService> logger,
    InvoiceMappingEngine engine,
    IApiKeyProvider apiKeyProvider,
    IInvoiceInforService invoiceInforService)
    : IInvoiceService
{
    private async Task<VnptInfor> GetVnptInfor()
    {
        var apiKey = apiKeyProvider.GetApiKey();
        if (apiKey is null) throw new Exception();
        var result = await invoiceInforService.GetByApiKeyAsync(apiKey, "vnpt");

        var res = JsonHelpers.Deserialize<VnptInfor>(result.Value) ?? throw new Exception();

        return res;
    }


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
            var xml = await TransferToPayload();

            ValidateBeforeSend(payload);
            var vnptInfo         = await GetVnptInfor();
            var plainCommandData = engine.TransformToXml(xml, payload.Data, "Invoices");


            var result = await client.ImportInvByPatternAsync(vnptInfo.Account, vnptInfo.AcPass, plainCommandData,
                                                              vnptInfo.Username, vnptInfo.Password, vnptInfo.Pattern,
                                                              vnptInfo.Serial, 0);

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


        var xml = await TransferToPayload("Mappings/vnpt.adjust.mapping.json.scriban");

        ValidateBeforeSend(payload);
        var vnptInfo = await GetVnptInfor();

        var plainCommandData = engine.TransformToXml(xml, payload.Data, "AdjustInv");

        try
        {
            var result = await clientBusiness.AdjustInvoiceActionAsync(vnptInfo.Account, vnptInfo.AcPass,
                                                                       plainCommandData,
                                                                       vnptInfo.Username, vnptInfo.Password,
                                                                       payload.RefKey, "",
                                                                       0, vnptInfo.Pattern, vnptInfo.Serial);

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


        var xml = await TransferToPayload("Mappings/vnpt.replace.mapping.json.scriban");

        ValidateBeforeSend(payload);
        var vnptInfo         = await GetVnptInfor();
        var plainCommandData = engine.TransformToXml(xml, payload.Data, "ReplaceInv");

        try
        {
            var result = await clientBusiness.ReplaceActionAssignedNoAsync(vnptInfo.Account, vnptInfo.AcPass,
                                                                           plainCommandData,
                                                                           vnptInfo.Username, vnptInfo.Password
                                                                           ,
                                                                           payload.RefKey, "",
                                                                           0, vnptInfo.Pattern, vnptInfo.Serial);

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

    private void ValidateBeforeSend(InvoiceContext payload)
    {
        payload.Data["amountInWords"] =
            Utils.Utils.NumberToText(decimal.Parse(payload.Data["amount"].ToString() ?? string.Empty));
    }
}