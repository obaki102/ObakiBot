using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace ObakiBot.Discord;

  public sealed class DiscordClient : IDisposable
    {
        private readonly GatewayClient _gatewayClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DiscordClient> _logger;

        public DiscordClient(
            GatewayClient gatewayClient,
            ApplicationCommandService<SlashCommandContext> slashCommandService,
            CommandService<CommandContext> commandService,
            IServiceProvider serviceProvider,
            ILogger<DiscordClient>  logger)
        {
            _gatewayClient = gatewayClient;
            _serviceProvider = serviceProvider;
            _logger = logger;

            DiscordEventContext = new DiscordEventContext(
                gatewayClient,
                slashCommandService,
                commandService,
                serviceProvider,
                logger
            );
        }
        
        public DiscordEventContext DiscordEventContext { get; }

        public async Task InitializeClientAsync()
        {
            _logger.LogInformation("Initializing Discord client...");
            DiscordEventContext.RegisterEvents();
            await DiscordEventContext.InitializeCommandsAsync();
            _logger.LogInformation("Discord client initialized successfully.");
        }

        public async Task StartClientAsync()
        {
            ArgumentNullException.ThrowIfNull(_gatewayClient);
            _logger.LogInformation("Starting Discord client...");
            await _gatewayClient.StartAsync();
            _logger.LogInformation("Discord client started.");
        }

        public async Task StopClientAsync()
        {
            _logger.LogInformation("Stopping Discord client...");
            await _gatewayClient.CloseAsync();
            _logger.LogInformation("Discord client stopped.");
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing Discord client...");
            _gatewayClient?.Dispose();
            _logger.LogInformation("Discord client disposed.");
        }
    }