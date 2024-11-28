using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;
using ObakiBot.Ai;
using ObakiBot.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services
    .AddDiscordGateway()
    .AddOllamaAiService()
    .AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext>()
    .AddDiscordGateway(options =>
    {
        options.Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent;
    });
   
var host = builder.Build();


host.AddModules(typeof(Program).Assembly);
host.UseGatewayEventHandlers();
await host.RunAsync();