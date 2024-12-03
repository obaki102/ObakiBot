using ObakiBot.Ai;
using ObakiBot.Ai.Services;
using ObakiBot.Api.Extensions;
using ObakiBot.Core.Models;

namespace ObakiBot.Api.Endpoints;

public class OllamaApiEndpoints : IEndpoint
{
    private const string tag = "Ollama Ai";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ai/ask-walter", async (string question, WalterAiService walterAiService) =>
            {
                Result<string> result = await walterAiService.AskWalterAsync(question);

                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
            })
            .WithTags(tag);

        app.MapGet("ai/site-synopsis", async (string url, SiteSynopsisService siteSynopsisService) =>
            {
                Result<string?> result = await siteSynopsisService.GetSiteSynopsisAsync(url);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
            })
            .WithTags(tag);
    }
}