using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using ObakiBot.Ai.Services;
#pragma warning disable SKEXP0070
namespace ObakiBot.Ai.Extensions;

public static class DependecyInjection
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
        services.AddHttpClient();
        services.AddSingleton<Kernel>();
        services.AddSingleton<WalterAiService>();
        services.AddSingleton<SiteSynopsisService>();

        return services;
    }
}