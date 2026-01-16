using Invoice.Extensions;
using InvoiceHub.Dto;
using InvoiceHub.Interfaces;
using InvoiceHub.WSPublicEHoaDon;

namespace InvoiceHub.Services;

public class BkavService(InvoiceMappingEngine engine, IConfiguration configuration) : IInvoiceService
{
    private readonly string _partnerGuid = configuration["Bkav:PartnerGuid"]
        ?? throw new ArgumentException("Bkav:PartnerGuid not found in configuration");

    private readonly string _partnerToken = configuration["Bkav:PartnerToken"] ??
        throw new ArgumentException("Bkav:PartnerToken not found in configuration");

    private readonly string _url = configuration["Bkav:Url"]
        ?? "https://wsdemo.ehoadon.vn/WSPublicEHoaDon.asmx";

    private async Task<string> TranferToPayload(InvoiceContext payload)
    {
        const string mappingPath = $"Mappings/bkav.mapping.json.scriban";

        if (!File.Exists(mappingPath))
            throw new Exception($"Mapping file not found: {mappingPath}");

        var mapping = await File.ReadAllTextAsync(mappingPath);

        var result = engine.TransformToJson(mapping, payload.Data);
        return result;
    }

    public async Task<InvoiceResponse> CreateInvoiceAsync(InvoiceContext payload)
    {
        
        
        payload.Data["cmdType"] = 100;
        payload.Data["refKey"]  = payload.RefKey ?? "";
        var plainCommandData = await TranferToPayload(payload);
        
        var encryptedData    = BkavEncryptHelper.EncryptCommandData(plainCommandData, _partnerToken);

        // payload.Data["InvoiceForm"] = 12;


        var client =
            new WSPublicEHoaDonSoapClient(WSPublicEHoaDonSoapClient.EndpointConfiguration.WSPublicEHoaDonSoap, _url);

        var response = await client.ExecuteCommandAsync(_partnerGuid, encryptedData);

        var encryptedResponse = response.Body.ExecuteCommandResult;

        var decryptedResponse = BkavEncryptHelper.DecryptCommandData(encryptedResponse, _partnerToken);

        return new InvoiceResponse();
    }

    public async Task<InvoiceResponse> AdjustInvoiceAsync(InvoiceContext payload)
    {
        
        payload.Data["cmdType"] = 121;
        payload.Data["refKey"]  = payload.RefKey ?? "";
        var plainCommandData = await TranferToPayload(payload);
        
        var encryptedData    = BkavEncryptHelper.EncryptCommandData(plainCommandData, _partnerToken);

        // payload.Data["InvoiceForm"] = 12;


        var client =
            new WSPublicEHoaDonSoapClient(WSPublicEHoaDonSoapClient.EndpointConfiguration.WSPublicEHoaDonSoap, _url);

        var response = await client.ExecuteCommandAsync(_partnerGuid, encryptedData);

        var encryptedResponse = response.Body.ExecuteCommandResult;

        var decryptedResponse = BkavEncryptHelper.DecryptCommandData(encryptedResponse, _partnerToken);

        return new InvoiceResponse();
    }

    public async Task<InvoiceResponse> ReplaceInvoiceAsync(InvoiceContext payload)
    {
        payload.Data["cmdType"] = 123;
        payload.Data["refKey"]  = payload.RefKey ?? "";
        var plainCommandData = await TranferToPayload(payload);
        
        var encryptedData    = BkavEncryptHelper.EncryptCommandData(plainCommandData, _partnerToken);



        var client =
            new WSPublicEHoaDonSoapClient(WSPublicEHoaDonSoapClient.EndpointConfiguration.WSPublicEHoaDonSoap, _url);

        var response = await client.ExecuteCommandAsync(_partnerGuid, encryptedData);

        var encryptedResponse = response.Body.ExecuteCommandResult;

        var decryptedResponse = BkavEncryptHelper.DecryptCommandData(encryptedResponse, _partnerToken);

        return new InvoiceResponse();
    }
}