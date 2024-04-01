using ChatbotAI.net.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

namespace ChatbotAI.net;

public static class KernelFactory
{

    public static Kernel Create(OpenAIOptions openAiOptions)
    {

        var builder = Kernel.CreateBuilder();


#pragma warning disable SKEXP0001 SKEXP0010 SKEXP0020 SKEXP0050
        builder.AddOpenAIChatCompletion(openAiOptions.Models.Chat, openAiOptions.ApiKey);
        builder.AddOpenAITextToAudio(openAiOptions.Models.Speech, openAiOptions.ApiKey);
        builder.AddOpenAIAudioToText(openAiOptions.Models.AudioTranscription);
#pragma warning disable SKEXP0010
        builder.AddOpenAITextEmbeddingGeneration(openAiOptions.Models.Embedding, openAiOptions.ApiKey);


        // Add Pluggins
#pragma warning disable SKEXP0050
        //builder.Plugins.AddFromPromptDirectory
        builder.Plugins.AddFromType<ConversationSummaryPlugin>();

#pragma warning restore SKEXP0001 SKEXP0010 SKEXP0020 SKEXP0050
        builder.Plugins.AddFromType<AuthorEmailPlanner>();
        builder.Plugins.AddFromType<GmailPlugin>();

        return  builder.Build();
    }
}