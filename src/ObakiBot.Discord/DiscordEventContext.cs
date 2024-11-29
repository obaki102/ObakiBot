using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace ObakiBot.Discord;

public class DiscordEventContext
{
    private readonly GatewayClient _gatewayClient;
    private readonly ApplicationCommandService<ApplicationCommandContext> _applicationCommandService;
    private readonly CommandService<CommandContext> _commandService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public DiscordEventContext(
        GatewayClient gatewayClient,
        ApplicationCommandService<ApplicationCommandContext> applicationCommandService,
        CommandService<CommandContext> commandService,
        IServiceProvider serviceProvider,
        ILogger logger)
    {
        _gatewayClient = gatewayClient;
        _applicationCommandService = applicationCommandService;
        _commandService = commandService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void RegisterEvents()
    {
        _logger.LogInformation("Registering gateway events...");
        _gatewayClient.Log += OnLogAsync;
        _gatewayClient.MessageCreate += OnMessageCreateAsync;
        _gatewayClient.InteractionCreate += OnInteractionCreateAsync;
    }

    public async Task InitializeCommandsAsync()
    {
        _logger.LogInformation("Initializing slash commands...");
        await _applicationCommandService.CreateCommandsAsync(_gatewayClient.Rest, _gatewayClient.Id);
        _logger.LogInformation("Slash commands initialized.");
    }

    public Func<LogMessage, ValueTask>? OnLogAsync { get; set; }

    private async ValueTask OnMessageCreateAsync(Message message)
    {
        if (message.Author.IsBot || !message.Content.StartsWith('!'))
            return;

        _logger.LogInformation("Processing message command: {Message}", message.Content);

        var result = await _commandService.ExecuteAsync(1, new CommandContext(message, _gatewayClient),_serviceProvider);
        
        if (result is IFailResult failResult)
        {
            try
            {
                await message.ReplyAsync(failResult.Message);
                _logger.LogWarning("Command failed: {ErrorMessage}", failResult.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reply to message command.");
            }
        }
    }

    private async ValueTask OnInteractionCreateAsync(Interaction interaction)
    {
        if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
        {
            _logger.LogWarning("Invalid interaction received.");
            return;
        }

        _logger.LogInformation("Processing slash command interaction.");

        var result = await _applicationCommandService.ExecuteAsync(
            new ApplicationCommandContext(applicationCommandInteraction, _gatewayClient),
            _serviceProvider
        );

        if (result is IFailResult failResult)
        {
            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
                _logger.LogWarning("Slash command failed: {ErrorMessage}", failResult.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send slash command response.");
            }
        }
    }
}