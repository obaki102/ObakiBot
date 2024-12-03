using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using ObakiBot.Ai.Plugins;

#pragma warning disable SKEXP0070
namespace ObakiBot.Ai.Services;

public class SiteSynopsisService : AiServiceBase
{
    public SiteSynopsisService(Kernel kernel, IServiceProvider serviceProvider)
        : base(kernel)
    {
        LoadPlugin<WebPullerPlugin>("WebPuller", serviceProvider);
    }

    public async Task<string> GetSiteSynopsisAsync(string url)
    {
       var ollamaSettings = new OllamaPromptExecutionSettings
            { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };


        var synopsis = await Kernel.InvokePromptAsync(
            $"""
             You are a bot assistant.  
             Your primary function is to fetch the content from {url}, summarize it in a clear, concise, and well-structured manner, similar to ChatGPT responses.  
             The summary should be free of formatting artifacts like quotes and should focus solely on the relevant information.
             """, new KernelArguments(ollamaSettings));
        return synopsis.ToString();
    }
}