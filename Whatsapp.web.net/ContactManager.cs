using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using Whatsapp.web.net.Domains;

namespace Whatsapp.web.net;

public class ContactManager : IContactManager
{
    private readonly IJavaScriptParser _parserFunctions;
    private IPage _pupPage;


    public ContactManager(IJavaScriptParser parserFunctions, IPage pupPage)
    {
        _parserFunctions = parserFunctions;
        _pupPage = pupPage;
    }

    public async Task<Contact> GetContactById(string contactId)
    {
        dynamic data = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getContactById"), contactId);

        return Contact.Create(data);
    }

    public async Task<bool> Block(Contact contact)
    {
        if (contact.IsGroup) return false;

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("blockContactById"), contact.Id.Serialized);

        contact.IsBlocked = true;
        return true;
    }

    public async Task<bool> Unblock(Contact contact)
    {
        if (contact.IsGroup) return false;

        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unblockContactById"), contact.Id.Serialized);

        contact.IsBlocked = false;
        return true;
    }

    public async Task<string?> GetAbout(Contact contact)
    {
        var about = await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getStatusContactById"), contact.Id.Serialized);

        if (about?["status"] is null || about["status"]!.Type != JTokenType.String)
        {
            return null;
        }

        return about["status"]!.Value<string>();
    }

    public async Task<string?> GetProfilePicUrl(string contactId)
    {
        dynamic profilePic = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getProfilePic"), contactId);

        return profilePic is not null ? profilePic.eurl : null;
    }

    public async Task<string> GetFormattedNumber(string number)
    {
        return await _pupPage.EvaluateFunctionAsync<string>(_parserFunctions.GetMethod("getFormattedNumber"), number);
    }

    public async Task<string> GetCountryCode(string number)
    {
        number = number.Replace(" ", "").Replace("+", "").Replace("@c.us", "");
        return await _pupPage.EvaluateFunctionAsync<string>(_parserFunctions.GetMethod("getCountryCode"), number);
    }


}