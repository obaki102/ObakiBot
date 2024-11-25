using NetCord.Services.ApplicationCommands;
using ObakiBot.Ai;
using ObakiBot.Core;

namespace ObakiBot.Discord.SlashCommands;

public class SlashCommandModule(OllamaAiService ollamaAiService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("pong", "Pong!")]
    public static string Pong() => "Ping!";

    [SlashCommand("ask-obaki-bot", "Ask obaki-bot command")]
    public async Task<string> AskObakiBotCommand(
        [SlashCommandParameter(Name = "question", Description = "Ask any question")]
        string @question)
    {
        var answer = await ollamaAiService.AskObakiBotAsync(question);
        return answer;
    }
}