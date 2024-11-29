using NetCord.Services.Commands;
using ObakiBot.Ai;

namespace ObakiBot.Discord.TextCommands;

public class TextCommandModule : CommandModule<CommandContext>
{
    private readonly OllamaAiService _ollamaAiService;

    public TextCommandModule(OllamaAiService olamaAiService)
    {
        _ollamaAiService = olamaAiService;
    }

    [Command("ask-obaki-bot", Priority = 0)]
    public async Task<string> AskObakiBot([CommandParameter(Remainder = true)] string question)
    {
        var answer = await _ollamaAiService.AskWalterAsync(question);
        return answer;
    }
}