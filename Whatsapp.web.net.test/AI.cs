using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Whatsapp.web.net.test;

public class AI
{
    private readonly Kernel _kernel;
    private readonly Dictionary<string, ChatHistory> _histories;

    public AI()
    {
        var builder = Kernel.CreateBuilder();

        builder.AddOpenAIChatCompletion("gpt-3.5-turbo", "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");
        _kernel = builder.Build();
        _histories = new Dictionary<string, ChatHistory>();
    }

    public async Task<string> Ask(string idId, string request)
    {
        if (!_histories.ContainsKey(idId)) _histories.Add(idId, new ChatHistory());
        var history = _histories[idId] ;
        var healthcareChat = _kernel.CreateFunctionFromPrompt(
            @"{{$history}}  
             User: {{$request}}  
             Asistente de viajes: "
        );

        var chatResult = _kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
            healthcareChat,
            new KernelArguments
            {
                { "request", request },
                { "history", string.Join("\n", history.Select(x => x.Role + ": " + x.Content)) }
            }
        );

        var response = new StringBuilder();
        await foreach (var chunk in chatResult)
        {
            request += chunk;
            response.Append(chunk);
        }
        history.AddUserMessage(request);
        history.AddUserMessage(response.ToString());
        return response.ToString();
    }
}