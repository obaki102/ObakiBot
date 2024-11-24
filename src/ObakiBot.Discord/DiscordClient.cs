using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace ObakiBot.Discord;

public sealed class DiscordClient : IDisposable
{
    private readonly GatewayClient _gatewayClient;
    private readonly ApplicationCommandService<ApplicationCommandContext> _applicationCommandService;

    public DiscordClient(GatewayClient gatewayClient,
             ApplicationCommandService<ApplicationCommandContext> applicationCommandService)
    {
        _gatewayClient = gatewayClient;
        _applicationCommandService = applicationCommandService;
    }

    public async Task InitializeClientAsync()
    {
        _gatewayClient.Log += OnLogAsync;
        _gatewayClient.MessageCreate += OnMessageCreate;
        _gatewayClient.InteractionCreate += async interaction =>
        {
            // Check if the interaction is an application command interaction
            if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
                return;

            // Execute the command
            var result = await _applicationCommandService.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, _gatewayClient));

            // Check if the execution failed
            if (result is not IFailResult failResult)
                return;

            // Return the error message to the user if the execution failed
            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        };
        
        await _applicationCommandService.CreateCommandsAsync(_gatewayClient.Rest, _gatewayClient.Id);
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