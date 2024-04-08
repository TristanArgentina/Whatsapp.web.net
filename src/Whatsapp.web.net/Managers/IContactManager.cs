using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net.Managers;

public interface IContactManager : IManager
{
    Task<Contact[]> Get();

    Task<Contact> Get(string contactId);

    Task<bool> Block(Contact contact);

    Task<bool> Unblock(Contact contact);

    Task<string?> GetAbout(string contactId);

    Task<string?> GetProfilePicUrl(string contactId);

    Task<string> GetFormattedNumber(string number);

    Task<string> GetCountryCode(string number);

    Task<Contact[]> GetBlocked();
}