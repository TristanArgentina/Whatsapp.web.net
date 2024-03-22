namespace Whatsapp.web.net.Domains;

public class Constants
{
    public const string WhatsWebURL = "https://web.whatsapp.com/";

    public enum Status
    {
        INITIALIZING = 0,
        AUTHENTICATING = 1,
        READY = 3
    };


    public enum GroupNotificationTypes
    {
        ADD,
        INVITE,
        REMOVE,
        LEAVE,
        SUBJECT,
        DESCRIPTION,
        PICTURE,
        ANNOUNCE,
        RESTRICT
    };

    public enum ChatTypes
    {
        SOLO,
        GROUP,
        UNKNOWN
    };
}