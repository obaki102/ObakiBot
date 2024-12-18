﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using ObakiBot.Ai.Extensions;

namespace ObakiBot.Discord.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDiscordDependencies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddLogging();
        services.AddOllamaAiService();
        services.AddSingleton<ApplicationCommandService<ApplicationCommandContext>>(_ =>
        {
            var applicationCommandService = new ApplicationCommandService<ApplicationCommandContext>();
            applicationCommandService.AddModules(Assembly.GetExecutingAssembly());
            return applicationCommandService;
        });

        services.AddSingleton<CommandService<CommandContext>>(_ =>
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