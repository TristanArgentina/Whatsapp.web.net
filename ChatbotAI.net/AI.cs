using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Numerics.Tensors;
using System.Text;
using ChatbotAI.net.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Milvus;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Milvus.Client;

namespace ChatbotAI.net;

public class AI : IAI
{
    private readonly string _apiKey;
    private readonly Kernel _kernel;
    private readonly ConcurrentDictionary<string, ChatHistory> _histories;

    public AI(string modelId, string apiKey)
    {
        _apiKey = apiKey;
        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(modelId, apiKey);


#pragma warning disable SKEXP0010
        builder.AddOpenAITextEmbeddingGeneration("text-embedding-ada-002", apiKey);
#pragma warning restore SKEXP0010

        _kernel = builder.Build();
        _histories = new ConcurrentDictionary<string, ChatHistory>();



        // TestEmbeding(apiKey);
    }

    private static void TestEmbeding(string apiKey)
    {
#pragma warning disable SKEXP0001 
#pragma warning disable SKEXP0010 
#pragma warning disable SKEXP0020 
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
        var embeddingGen = new OpenAITextEmbeddingGenerationService("text-embedding-3-small", apiKey);
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
        var memoryStore = new MilvusMemoryStore(milvusClient);
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
        var history = _histories.GetOrAdd(idId, []);
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

    public async Task<string> ConvertToText(string filePath, MemoryStream memoryStream)
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        //using var memoryStream = new MemoryStream();
        //await using var fs = File.OpenRead(filePath);
        //await fs.CopyToAsync(memoryStream);
        var content = new MultipartFormDataContent
        {
            { new StringContent("whisper-1"), "model" },
            { new StringContent("srt"), "response_format" },
            { new ByteArrayContent(memoryStream.ToArray()), "file", filePath }
        };
        var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", content);
        return await response.Content.ReadAsStringAsync();
    }
}