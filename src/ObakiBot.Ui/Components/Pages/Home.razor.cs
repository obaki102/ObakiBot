using Microsoft.AspNetCore.Components;
using NetCord.Gateway;
using ObakiBot.Discord;

namespace ObakiBot.Ui.Components.Pages;

public partial class Home : ComponentBase, IDisposable
{
    private readonly DiscordClient _discordClient;
    private readonly List<string> _logs = [];
    private bool _isClientstarted;

    public Home(DiscordClient discordClient)
    {
        _discordClient = discordClient;
    }

    protected override async Task OnInitializedAsync()
    {
        _isClientstarted = _discordClient.IsClientStarted;
        _discordClient.DiscordEventContext.OnLogAsync += LogTask;
        await _discordClient.InitializeClientAsync();
    }

    private async Task StartClient()
    {
        _isClientstarted = true;
        await _discordClient.StartClientAsync();
    }

    private async Task StopClient()
    {
        _isClientstarted = false;
        await _discordClient.StopClientAsync();
    }

    private async ValueTask LogTask(LogMessage logMessage)
    {
        var log = logMessage.ToString();
        _logs.Add(log);
        await InvokeAsync(StateHasChanged);
    }

    private async ValueTask OnMessageReceived(Message message)
    {
        _logs.Add(message.Content);
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _discordClient?.Dispose();
    }
}