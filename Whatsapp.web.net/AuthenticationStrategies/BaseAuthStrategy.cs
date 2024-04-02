namespace Whatsapp.web.net.AuthenticationStrategies;

public abstract class BaseAuthStrategy
{
    protected Client? Client;
    protected string? SessionName;
    protected WhatsappOptions Options;

    public virtual void Setup(Client client, WhatsappOptions options)
    {
        Client = client;
        Options = options;
    }

    public virtual Task BeforeBrowserInitialized()
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterBrowserInitialized()
    {
        return Task.CompletedTask;
    }

    public virtual async Task<(bool Failed, bool Restart, object FailureEventPayload)> OnAuthenticationNeeded()
    {
        return (Failed: false, Restart: false, FailureEventPayload: null);
    }

    public virtual Task<object> GetAuthEventPayload()
    {
        return Task.FromResult<object>(null);
    }

    public virtual Task AfterAuthReady()
    {
        return Task.CompletedTask;
    }

    public virtual Task Disconnect()
    {
        return Task.CompletedTask;
    }

    public virtual Task Destroy()
    {
        return Task.CompletedTask;
    }

    public virtual Task Logout()
    {
        return Task.CompletedTask;
    }
}