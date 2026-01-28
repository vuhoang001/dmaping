using InvoiceHub.Dto;
using InvoiceHub.Extensions;
using InvoiceHub.Interfaces;
using InvoiceHub.Models;
using InvoiceHub.Services.InvoiceInformation;
using InvoiceHub.Utils;
using InvoiceHub.WSPublicEHoaDon;

namespace InvoiceHub.Services;

public class BkavService(
    InvoiceMappingEngine engine,
    IConfiguration configuration,
    IInvoiceInforService invoiceInforService,
    IApiKeyProvider apiKeyProvider) : IInvoiceService
{
    private async Task<BkavInfor> GetBkavInfor()
    {
        var apiKey = apiKeyProvider.GetApiKey();
        if (apiKey is null) throw new Exception();
        var result = await invoiceInforService.GetByApiKeyAsync(apiKey, "bkav");

        var res = JsonHelpers.Deserialize<BkavInfor>(result.Value) ?? throw new Exception();

        return res;
    }

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
        var bkavInfo         = await SetupBeforeSend(100, payload);
        var plainCommandData = await TranferToPayload(payload);


        var encryptedData = BkavEncryptHelper.EncryptCommandData(plainCommandData, bkavInfo.PartnerToken);

        // payload.Data["InvoiceForm"] = 12;


        var client =
            new WSPublicEHoaDonSoapClient(WSPublicEHoaDonSoapClient.EndpointConfiguration.WSPublicEHoaDonSoap, _url);

        var response = await client.ExecuteCommandAsync(bkavInfo.PartnerGuid, encryptedData);

        var encryptedResponse = response.Body.ExecuteCommandResult;

        var decryptedResponse = BkavEncryptHelper.DecryptCommandData(encryptedResponse, bkavInfo.PartnerToken);

        return new InvoiceResponse
        {
            XValue    = decryptedResponse,
            IsSuccess = true,
            Message   = null
        };
    }

    public async Task<InvoiceResponse> AdjustInvoiceAsync(InvoiceContext payload)
    {
        var bkavInfo = await SetupBeforeSend(121, payload);

        var plainCommandData = await TranferToPayload(payload);

        var encryptedData = BkavEncryptHelper.EncryptCommandData(plainCommandData, bkavInfo.PartnerToken);

        // payload.Data["InvoiceForm"] = 12;


        var client =
            new WSPublicEHoaDonSoapClient(WSPublicEHoaDonSoapClient.EndpointConfiguration.WSPublicEHoaDonSoap, _url);

        var response = await client.ExecuteCommandAsync(bkavInfo.PartnerGuid, encryptedData);

        var encryptedResponse = response.Body.ExecuteCommandResult;

        var decryptedResponse = BkavEncryptHelper.DecryptCommandData(encryptedResponse, bkavInfo.PartnerToken);

        return new InvoiceResponse
        {
            XValue    = decryptedResponse,
            IsSuccess = true,
            Message   = null
        };
    }

    public async Task<InvoiceResponse> ReplaceInvoiceAsync(InvoiceContext payload)
    {
        var bkavInfo         = await SetupBeforeSend(123, payload);
        var plainCommandData = await TranferToPayload(payload);

        var encryptedData = BkavEncryptHelper.EncryptCommandData(plainCommandData, bkavInfo.PartnerToken);


        var client =
            new WSPublicEHoaDonSoapClient(WSPublicEHoaDonSoapClient.EndpointConfiguration.WSPublicEHoaDonSoap, _url);

        var response = await client.ExecuteCommandAsync(bkavInfo.PartnerGuid, encryptedData);

        var encryptedResponse = response.Body.ExecuteCommandResult;

        var decryptedResponse = BkavEncryptHelper.DecryptCommandData(encryptedResponse, bkavInfo.PartnerToken);

        return new InvoiceResponse
        {
            XValue    = decryptedResponse,
            IsSuccess = true,
            Message   = null
        };
    }

    private async Task<BkavInfor> SetupBeforeSend(int type, InvoiceContext payload)
    {
        var bkavInfo = await GetBkavInfor();

        payload.Data["cmdType"]       = type;
        payload.Data["refKey"]        = payload.RefKey ?? "";
        payload.Data["invoiceForm"]   = bkavInfo.InvoiceForm;
        payload.Data["invoiceSerial"] = bkavInfo.InvoiceSerial;
        return bkavInfo;
    }
}