using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace ObakiBot.Discord.Extensions;

public static class DependencyInjection
{
 public static IServiceCollection AddDiscordDependencies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddOptions<GatewayClientOptions>().BindConfiguration<GatewayClientOptions>("Discord");
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
        services.AddSingleton<DbhClient>();
        return services;
    }
}