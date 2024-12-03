using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using ObakiBot.Ai.Plugins;

#pragma warning disable SKEXP0070


namespace ObakiBot.Ai;

public class OllamaAiService
{
    private readonly IChatCompletionService _chatCompletionService;
    private readonly Kernel _kernel;
    private readonly ChatHistory _chatHistory = [];

    public OllamaAiService(Kernel kernel, IServiceProvider serviceProvider, IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        _chatHistory.AddSystemMessage("""
                                          You are Walter White, a brilliant, calculated, and commanding individual. 
                                          When a question is asked, you answer it with clarity, precision, and an air of authority. 
                                          Do not sugarcoat your responses—be direct, confident, and assertive, as if every word you say matters. 
                                          Stay focused on the task and do not waste time on unnecessary details or pleasantries. 
                                      """);
        _kernel = kernel;
        _kernel.Plugins.AddFromType<WebPullerPlugin>("WebPuller",serviceProvider);
    }

    public async Task<string> AskWalterAsync(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
            return "Please make sure you enter a valid question.";
        _chatHistory.AddUserMessage(question);

        var result = await _chatCompletionService.GetChatMessageContentsAsync(_chatHistory);
        string resultText = string.Join(" ", result);
        return resultText;
    }

    public async Task<string?> SiteSynopsisAsync(string url)
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        var ollamaSettings = new OllamaPromptExecutionSettings

            { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
        
        var instruction =
            $"""
             You are a bot assistant.  
             Your primary function is to fetch the content from {url}, summarize it in a clear, concise, and well-structured manner, similar to ChatGPT responses.  
             The summary should be free of formatting artifacts like quotes and should focus solely on the relevant information.
             """;
        var chatResult =
            await chatCompletionService.GetChatMessageContentAsync(instruction, ollamaSettings, _kernel);
        return chatResult.Content;
    }
}

public static class UseOllamaAiServiceExtension
{
    public static IServiceCollection AddOllamaAiService(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        var serviceProvider = services.BuildServiceProvider();
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var ollamaConnectionString = config.GetConnectionString("ollama-llama3-2");

        var connectionBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = ollamaConnectionString
        };

        var endpoint = connectionBuilder["Endpoint"]?.ToString() ?? string.Empty;
        var parsedModel = connectionBuilder["Model"]?.ToString() ?? string.Empty;
        Console.WriteLine(endpoint + ":" + parsedModel);
        services.AddOllamaChatCompletion(parsedModel, new Uri(endpoint));
        //services.AddOllamaChatCompletion("llama3.2", new Uri("http://localhost:11434"));
        services.AddSingleton<Kernel>();
        services.AddSingleton<OllamaAiService>();

        return services;
    }
}