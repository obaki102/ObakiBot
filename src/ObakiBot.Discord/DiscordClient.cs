using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using ObakiBot.Ai;

namespace ObakiBot.Discord;

public sealed class DiscordClient : IDisposable
{
    private readonly GatewayClient _gatewayClient;
    private readonly ApplicationCommandService<SlashCommandContext> _slashCommandService;
    private readonly CommandService<CommandContext> _commandService;
    private readonly IServiceProvider _serviceProvider;

    public DiscordClient(GatewayClient gatewayClient,
             ApplicationCommandService<SlashCommandContext> slashCommandService,
             CommandService<CommandContext> commandService,
             IServiceProvider serviceProvider)
    {
        _gatewayClient = gatewayClient;
        _slashCommandService = slashCommandService;
        _commandService = commandService;
        _serviceProvider  = serviceProvider;
        
    }

    public async Task InitializeClientAsync()
    {
        _gatewayClient.Log += OnLogAsync;
        _gatewayClient.MessageCreate += OnMessageCreate;
        _gatewayClient.InteractionCreate += async interaction =>
        {
            var result = await (interaction switch
            {
                SlashCommandInteraction slashCommandInteraction => _slashCommandService.ExecuteAsync(new SlashCommandContext(slashCommandInteraction,_gatewayClient),_serviceProvider),
                _ => throw new("Invalid interaction."),
            });

            if (result is not IFailResult failResult)
                return;

            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
            }
            catch
            {
            }
        };
        _gatewayClient.MessageCreate += async message =>
        {
            // Check if the message is a command (starts with '!' and is not from a bot)
            if (!message.Content.StartsWith('!') || message.Author.IsBot)
                return;

            // Execute the command
            var result = await _commandService.ExecuteAsync(prefixLength: 1, new CommandContext(message, _gatewayClient));

            // Check if the execution failed
            if (result is not IFailResult failResult)
                return;

            // Return the error message to the user if the execution failed
            try
            {
                await message.ReplyAsync(failResult.Message);
            }
            catch
            {
            }
        };
        
        await _slashCommandService.CreateCommandsAsync(_gatewayClient.Rest, _gatewayClient.Id);
    }

    public async Task StartClientAsync()
    {
        ArgumentNullException.ThrowIfNull(_gatewayClient);
        await _gatewayClient.StartAsync();
    }

    public async Task StopClientAsync()
    {
        await _gatewayClient.CloseAsync();
    }

    public Func<LogMessage, ValueTask>? OnLogAsync { get; set; }
    public Func<Message, ValueTask>? OnMessageCreate { get; set; }

    public void Dispose()
    {
        _gatewayClient?.Dispose();
    }
}