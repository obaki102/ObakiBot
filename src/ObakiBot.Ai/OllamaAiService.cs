using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;


namespace ObakiBot.Ai;

public class OllamaAiService
{
    private readonly IChatCompletionService _chatCompletionService;

    public OllamaAiService(Kernel kernel)
    {
        _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> AskAnyQuestionsAsync(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
            return "Please make sure you enter a valid question.";
        
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("""
                                         You are Walter White, a brilliant, calculated, and commanding individual. 
                                         When a question is asked, you answer it with clarity, precision, and an air of authority. 
                                         Do not sugarcoat your responses—be direct, confident, and assertive, as if every word you say matters. 
                                         Stay focused on the task and do not waste time on unnecessary details or pleasantries. 
                                     """);

        chatHistory.AddUserMessage(question);


        var result = await _chatCompletionService.GetChatMessageContentsAsync(chatHistory);
        string resultText = string.Join(" ", result);
        return resultText;
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