using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using Whatsapp.web.net.Domains;
using Whatsapp.web.net.scripts;

namespace Whatsapp.web.net.Managers;

public class ContactManager : IContactManager
{
    private readonly IJavaScriptParser _parserFunctions;
    private readonly IPage _pupPage;


    public ContactManager(IJavaScriptParser parserFunctions, IPage pupPage)
    {
        _parserFunctions = parserFunctions;
        _pupPage = pupPage;
    }

    public async Task<Contact> Get(string contactId)
    {
        dynamic data = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getContactById"), contactId).Result;

        return Contact.Create(data);
    }

    public async Task<bool> Block(Contact contact)
    {
        if (contact.IsGroup) return false;
        Block(contact.Id._serialized);
        // contact.IsBlocked = true;
        return true;
    }

    public async Task<Contact[]> GetBlocked()
    {
        dynamic data = _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getBlockedContacts")).Result;

        var dataList = new List<dynamic>((JArray)data);

        return dataList.Select(d => new Contact(d)).ToArray();
    }

    public async void Block(string contactId)
    {
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("blockContactById"), contactId);
    }


    public async Task<bool> Unblock(Contact contact)
    {
        if (contact.IsGroup) return false;
        await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("unblockContactById"), contact.Id._serialized);
        //contact.IsBlocked = false;
        return true;
    }

    public async Task<string?> GetAbout(string contactId)
    {
        var about = await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getStatusContactById"), contactId);

        if (about?["status"] is null || about["status"]!.Type != JTokenType.String)
        {
            return null;
        }

        return about["status"]!.Value<string>();
    }

    public async Task<string?> GetProfilePicUrl(string contactId)
    {
        dynamic profilePic = await _pupPage.EvaluateFunctionAsync(_parserFunctions.GetMethod("getProfilePic"), contactId);

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

    public async Task<Contact[]> Get()
    {
        var data = _pupPage.EvaluateFunctionAsync<dynamic>(_parserFunctions.GetMethod("getContacts")).Result;

        var dataList = new List<dynamic>((JArray)data);

        return dataList.Select(d => new Contact(d)).ToArray();
    }

}