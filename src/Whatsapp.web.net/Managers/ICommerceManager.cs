using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Managers;

public interface ICommerceManager : IManager
{
    Task<Order?> GetOrderAsync(string msgType, string orderId, string token, string chatId);

    Task<ProductMetadata?> GetProductMetadataById(string productId);

    Task<Payment?> GetPayment(MessageId msgId, string msgType);
}