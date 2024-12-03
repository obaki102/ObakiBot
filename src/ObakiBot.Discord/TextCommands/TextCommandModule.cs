using NetCord.Services.Commands;
using ObakiBot.Ai;
using ObakiBot.Ai.Services;

namespace ObakiBot.Discord.TextCommands;

public class TextCommandModule : CommandModule<CommandContext>
{
    private readonly WalterAiService _walterAiService;

    public TextCommandModule(WalterAiService walterAiService)
    {
        _walterAiService = walterAiService;
    }

    [Command("ask-obaki-bot", Priority = 0)]
    public async Task<string> AskObakiBot([CommandParameter(Remainder = true)] string question)
    {
        var answer = await _walterAiService.AskWalterAsync(question);
        return answer;
    }
}