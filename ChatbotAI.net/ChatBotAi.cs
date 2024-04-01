using System.Net.Http.Headers;
using System.Numerics.Tensors;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Milvus;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Milvus.Client;

namespace ChatbotAI.net;

public class ChatBotAi : IChatBotAI
{
    private readonly OpenAIOptions _openAiOptions;
    private readonly Kernel _kernel;
    private ChatHistory _history;
    private readonly OpenAIClient _openai;
    private readonly IChatCompletionService _chatCompletionService;

    public ChatBotAi(OpenAIOptions openAiOptions, Kernel kernel)
    {
        _openAiOptions = openAiOptions;
        _kernel = kernel;
        CreateHistory();
        _openai = new OpenAIClient(openAiOptions.ApiKey);
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    private void CreateHistory()
    {
        _history = new ChatHistory(@"
        Eres un asistente amigable, que vas a responder honestamente a las preguntas.
        Tu nombre va a ser Venus.
        No desperdicies palabras, utiliza oraciones cortas, claras y completas.
        No utilice viñetas ni guiones.
        Usa voz activa.
        Maximizar el detalle, el significado enfoque en el contenido");
    }

    private static void TestEmbeding(string apiKey)
    {

        var input = "What is an amphibian?";
        string[] examples =
        {
            "What is an amphibian?",
            "Cos'è un anfibio?",
            "A frog is an amphibian.",
            "Frogs, toads, and salamanders are all examples.",
            "Amphibians are four-limbed and ectothermic vertebrates of the class Amphibia.",
            "They are four-limbed and ectothermic vertebrates.",
            "A frog is green.",
            "A tree is green.",
            "It's not easy bein' green.",
            "A dog is a mammal.",
            "A dog is a man's best friend.",
            "You ain't never had a friend like me.",
            "Rachel, Monica, Phoebe, Joey, Chandler, Ross",
        };
#pragma warning disable SKEXP0010
        var embeddingGen = new OpenAITextEmbeddingGenerationService("text-embedding-3-small", apiKey);
#pragma warning restore SKEXP0010
        var inputEmbedding = embeddingGen.GenerateEmbeddingsAsync([input]).Result[0];
        var exampleEmbeddings = embeddingGen.GenerateEmbeddingsAsync(examples).Result;
        var similarity = exampleEmbeddings.Select(e => TensorPrimitives.CosineSimilarity(e.Span, inputEmbedding.Span)).ToArray();
        similarity.AsSpan().Sort(examples.AsSpan(), (f1, f2) => f2.CompareTo(f1));
        Console.WriteLine("Similarity Example");
        for (var i = 0; i < similarity.Length; i++)
        {
            Console.WriteLine($"{similarity[i]:F6}   {examples[i]}");
        }



        var endpoint = new Uri("http://localhost:19530");
        var milvusClient = new MilvusClient(endpoint);
#pragma warning disable SKEXP0020
        var memoryStore = new MilvusMemoryStore(milvusClient);
#pragma warning restore SKEXP0020
        var collectionName = "test-autino";
        //memoryStore.CreateCollectionAsync(collectionName);
        //var memoryRecordMetadata = new MemoryRecordMetadata(false, "1", "text", "description", "externalSourceName", "additionalMetadata");
        //foreach (var exampleEmbedding in exampleEmbeddings)
        //{
        //    memoryStore.UpsertAsync(collectionName, new MemoryRecord(memoryRecordMetadata, exampleEmbedding, null));

        //}

        var r = milvusClient.GetCollection(collectionName).SearchAsync("", [inputEmbedding], SimilarityMetricType.Hamming, 10);



#pragma warning restore SKEXP0010 SKEXP0020 SKEXP0001
    }

    public async Task<string> Ask(string idId, string request)
    {
        var settings = new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        _history.AddUserMessage(request);
        var result = _chatCompletionService.GetStreamingChatMessageContentsAsync(_history, settings, _kernel);
        var fullMessage = string.Empty;
        await foreach (var content in result)
        {
            fullMessage += content.Content;
        }
        _history.AddAssistantMessage(fullMessage);
        return fullMessage;
    }

    public async Task<string> ConvertToText(string filePath, MemoryStream memoryStream)
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiOptions.ApiKey);

        var content = new MultipartFormDataContent
        {
            { new StringContent(_openAiOptions.Models.AudioTranscription), "model" },
            { new StringContent("srt"), "response_format" },
            { new ByteArrayContent(memoryStream.ToArray()), "file", filePath }
        };
        var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", content);
        return await response.Content.ReadAsStringAsync();
    }

    public byte[] GenerateSpeechFromText(string text)
    {
        var openai = new OpenAIClient(_openAiOptions.ApiKey);
        var speechGenerationOptions = new SpeechGenerationOptions()
        {
            DeploymentName = _openAiOptions.Models.Speech,
            Input = text,
            Voice = new SpeechVoice("shimmer"),
            ResponseFormat = new SpeechGenerationResponseFormat("opus"),
            Speed = 1
        };
        var audio = openai.GenerateSpeechFromText(speechGenerationOptions);
        return audio.Value.ToArray();
    }

    public string GetAudioTranscription(byte[] audio)
    {
        var binaryData = BinaryData.FromBytes(audio);
        return GetAudioTranscription(binaryData);
    }

    public string GetAudioTranscription(BinaryData binaryData)
    {
        var audioTranscriptionOptions = new AudioTranscriptionOptions()
        {
            AudioData = binaryData,
            DeploymentName = _openAiOptions.Models.AudioTranscription
        };
        var audioTranscription = _openai.GetAudioTranscription(audioTranscriptionOptions);
        return audioTranscription.Value.Text;
    }
}