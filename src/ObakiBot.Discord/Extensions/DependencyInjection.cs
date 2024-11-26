using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using ObakiBot.Ai;

namespace ObakiBot.Discord.Extensions;

public static class DependencyInjection
{
 public static IServiceCollection AddDiscordDependencies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddOllamaAiService();
        services.AddSingleton<ApplicationCommandService<SlashCommandContext>>(static serviceProvider =>
        {
            var slashCommandService = new ApplicationCommandService<SlashCommandContext>();
            slashCommandService.AddModules(Assembly.GetExecutingAssembly());
            return slashCommandService;
        });
        
        services.AddSingleton<CommandService<CommandContext>>(static serviceProvider =>
        {
            var textCommandService = new CommandService<CommandContext>();
            textCommandService.AddModules(Assembly.GetExecutingAssembly());
            return textCommandService;
        });
        
        services.AddOptions<GatewayClientOptions>().BindConfiguration("Discord");
        services.AddSingleton<GatewayClient>(static serviceProvider =>
        {
            var gatewayClientOptions =
                serviceProvider.GetRequiredService<IOptions<GatewayClientOptions>>().Value;
            var config = new GatewayClientConfiguration()
            {
                Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent,
            };
            return new GatewayClient(new BotToken(gatewayClientOptions.Token ?? string.Empty), config);
        });
       
        services.AddSingleton<DiscordClient>();
        return services;
    }
}