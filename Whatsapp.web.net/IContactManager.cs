using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net;

public interface IContactManager : IManager
{
    Task<Contact> GetContactById(string contactId);
    Task<bool> Block(Contact contact);
    Task<bool> Unblock(Contact contact);
    Task<string?> GetAbout(Contact contact);
    Task<string?> GetProfilePicUrl(string contactId);
    Task<string> GetFormattedNumber(string number);
    Task<string> GetCountryCode(string number);
}