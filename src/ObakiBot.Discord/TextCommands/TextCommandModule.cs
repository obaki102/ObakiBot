using NetCord.Services;
using NetCord.Services.Commands;
using ObakiBot.Ai;

namespace ObakiBot.Discord.TextCommands;

public class TextCommandModule(OllamaAiService ollamaAiService)  :  CommandModule<CommandContext>
{
    [Command("ask-obaki-bot", Priority = 0)]
    public  async Task<string> AskObakiBot([CommandParameter(Remainder = true)] string question)
    {
        var answer = await ollamaAiService.AskWalterAsync(question);
        return answer;
    }
    
}