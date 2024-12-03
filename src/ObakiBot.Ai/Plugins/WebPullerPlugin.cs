using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ObakiBot.Ai.Plugins;

public class WebPullerPlugin

{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebPullerPlugin(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [KernelFunction, Description("Pull content from web")]
    public async Task<string> PullContent(
        [Description("The url whree to pull the content")]
        string url)
    {
        using var client = _httpClientFactory.CreateClient();
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}