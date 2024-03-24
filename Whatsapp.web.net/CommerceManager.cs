using PuppeteerSharp;
using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net;

public class CommerceManager : ICommerceManager
{
    private readonly IJavaScriptParser _parserFunctions;
    private IPage _pupPage;


    public CommerceManager(IJavaScriptParser parserFunctions, IPage pupPage)
    {
        _parserFunctions = parserFunctions;
        _pupPage = pupPage;
    }

    public async Task<Order?> GetOrderAsync(string msgType, string orderId, string token, string chatId)
    {
        if (msgType != MessageTypes.ORDER) return null;
        var result = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getOrderDetail"), orderId, token, chatId);
        return result == null ? null : new Order(result);
    }


    public async Task<ProductMetadata?> GetProductMetadataById(string productId)
    {
        var result = await _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getProductMetadataById"), productId);
        return result is not null ? new ProductMetadata(result) : null;
    }

    public async Task<Payment?> GetPayment(MessageId msgId, string msgType)
    {
        if (msgType != MessageTypes.PAYMENT) return null;
        var data = await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getMessageSerialized"), msgId);
        return new Payment(data);
    }
}