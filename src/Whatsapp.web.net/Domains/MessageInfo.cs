namespace Whatsapp.web.net.Domains;

public class MessageInfo
{
    public DeliveryInfo[] Delivery { get; set; }
    public int DeliveryRemaining { get; set; }
    public PlayedInfo[] Played { get; set; }
    public int PlayedRemaining { get; set; }
    public ReadInfo[] Read { get; set; }
    public int ReadRemaining { get; set; }
}