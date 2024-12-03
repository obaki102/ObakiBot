using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ObakiBot.Ai;
using ObakiBot.Ai.Services;

namespace ObakiBot.Discord.SlashCommands;

public class SlashCommandModule
    : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly WalterAiService _walterAiService;

    public SlashCommandModule(WalterAiService walterAiService)
    {
        _walterAiService = walterAiService;
    }

    [SlashCommand("ping", "Ping!")]
    public async Task Pong()
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        await Task.Delay(10000);
        await Context.Interaction.ModifyResponseAsync(message => message.Content = "pong");
    }

    [SlashCommand("ask-obaki-bot", "Ask obaki-bot command")]
    public async Task AskObakiBotCommand(
        [SlashCommandParameter(Name = "question", Description = "Ask any question")]
        string @question)
    {
     
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        var answer = await _walterAiService.AskWalterAsync(question);
        await Context.Interaction.ModifyResponseAsync(message =>
        {
            message.Content = answer;
            message.AllowedMentions = AllowedMentionsProperties.None;

        });
    }

    [SlashCommand("user", "Shows user info")]
    public async Task UserAsync(User user)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(user.Id.ToString()));
    }
}