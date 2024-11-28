using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace ObakiBot.Discord;

public sealed class DiscordClient : IDisposable
{
    private readonly GatewayClient _gatewayClient;
    private readonly ILogger<DiscordClient> _logger;
    private bool _isClientStarted;
    public DiscordEventContext DiscordEventContext { get; }
    public bool IsClientStarted => _isClientStarted;

    public DiscordClient(
        GatewayClient gatewayClient,
        ApplicationCommandService<ApplicationCommandContext> applicationCommandService,
        CommandService<CommandContext> commandService,
        IServiceProvider serviceProvider,
        ILogger<DiscordClient> logger)
    {
        _gatewayClient = gatewayClient ?? throw new ArgumentNullException(nameof(gatewayClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        DiscordEventContext = CreateEventContext(gatewayClient, applicationCommandService, commandService, serviceProvider, logger);
    }

    public async Task InitializeClientAsync()
    {
         _logger.LogInformation("Initializing Discord client...");
        DiscordEventContext.RegisterEvents();
        await DiscordEventContext.InitializeCommandsAsync();
         _logger.LogInformation("Discord client initialized successfully.");
    }

    public async Task StartClientAsync()
    {
        EnsureClientNotNull();
        if (_isClientStarted)
        {
            _logger.LogInformation("Discord client is already started. Start operation skipped.");
            return;
        }

        EnsureClientNotNull();

        _logger.LogInformation("Starting Discord client...");
        await _gatewayClient.StartAsync();
        _isClientStarted = true;
        _logger.LogInformation("Discord client started.");
    }

    public async Task StopClientAsync()
    {
        if (!_isClientStarted)
        {
            _logger.LogInformation("Discord client is not running. Stop operation skipped.");
            return;
        }

        _logger.LogInformation("Stopping Discord client...");
        await _gatewayClient.CloseAsync();
        _isClientStarted = false;
        _logger.LogInformation("Discord client stopped.");
    }

 
    private DiscordEventContext CreateEventContext(
        GatewayClient gatewayClient,
        ApplicationCommandService<ApplicationCommandContext> applicationCommandService,
        CommandService<CommandContext> commandService,
        IServiceProvider serviceProvider,
        ILogger logger)
    {
        return new DiscordEventContext(gatewayClient, applicationCommandService, commandService, serviceProvider, logger);
    }

    private void EnsureClientNotNull()
    {
        if (_gatewayClient == null)
        {
            throw new InvalidOperationException("GatewayClient is not initialized.");
        }
    }
    
    public void Dispose()
    {
        if (_isClientStarted)
        {
            _logger.LogInformation("Stopping Discord client before disposal...");
            _gatewayClient.CloseAsync();
            _isClientStarted = false;
        }

        _logger.LogInformation("Disposing Discord client...");
        _gatewayClient?.Dispose();
        _logger.LogInformation("Discord client disposed.");
    }

}
