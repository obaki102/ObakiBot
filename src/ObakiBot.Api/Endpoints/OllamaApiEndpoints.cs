using ObakiBot.Ai;
using ObakiBot.Api.Extensions;
using ObakiBot.Core.Models;

namespace ObakiBot.Api.Endpoints;

public class OllamaApiEndpoints : IEndpoint
{
    private const string tag = "Ollama Ai";
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ai/ask-walter", async (string question, OllamaAiService ollamaAiService) =>
            {
                Result<string> result = await ollamaAiService.AskWalterAsync(question);

                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
            })
            .WithTags(tag);
        
    }
}