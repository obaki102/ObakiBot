using NetCord;
using NetCord.Gateway;

namespace ObakiBot.Discord;

// public class DbhClient : IDisposable
// {
//     private GatewayClient? _gatewayClient;
//
//     public void CreateClient(string token)
//     {
//         var botToken = new BotToken(token);
//         if (_gatewayClient is null)
//         {
//             _gatewayClient = new GatewayClient(botToken, new GatewayClientConfiguration()
//             {
//                 Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent,
//             });
//             _gatewayClient.Log += OnLogAsync;
//             _gatewayClient.MessageCreate += OnMessageCreate;
//         }
//     }
//
//     public async Task StartClientAsync()
//     {
//         ArgumentNullException.ThrowIfNull(_gatewayClient);
//         await _gatewayClient.StartAsync();
//     }
//
//     public async Task StopClientAsync()
//     {
//         if (_gatewayClient is not null)
//         {
//             await _gatewayClient.CloseAsync();
//         }
//     }
//
//     public Func<LogMessage, ValueTask>? OnLogAsync { get; set; }
//     public Func<Message, ValueTask>? OnMessageCreate { get; set; }
//
//     public void Dispose()
//     {
//         _gatewayClient?.Dispose();
//     }
// }

public class DbhClient : IDisposable
{
    private readonly GatewayClient _gatewayClient;

    public DbhClient(GatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
    }

    public void InitializeEvents()
    {
        _gatewayClient.Log += OnLogAsync;
        _gatewayClient.MessageCreate += OnMessageCreate;
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