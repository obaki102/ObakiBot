using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ObakiBot.Ai.Plugins;

public class WebPullerPlugin
{
    //There is an issue with the plugin when using DI
    private static readonly HttpClient client = new();

    [KernelFunction, Description("Pull content from web")]
    public async Task<string> PullContent(
        [Description("The url whree to pull the content")]
        string url)
    {
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}