using NetCord.Services.ApplicationCommands;

namespace ObakiBot.Discord.SlashCommands;

public class PingPongModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("pong", "Pong!")]
    public static string Pong() => "Ping!";
}