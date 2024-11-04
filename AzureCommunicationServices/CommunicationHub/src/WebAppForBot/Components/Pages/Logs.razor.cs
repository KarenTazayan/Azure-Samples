using Microsoft.AspNetCore.Components;

namespace WebAppForBot.Components.Pages;

public partial class Logs : IDisposable
{
    [Inject]
    private WebLogService LogService { get; set; } = null!;

    private readonly List<string> _logs = [];

    protected override void OnInitialized()
    {
        // Initialize logs from the LogService
        _logs.AddRange(LogService.GetLogs());

        // Subscribe to the OnLogAdded event
        LogService.OnLogAdded += UpdateLogs;
    }

    private async void UpdateLogs()
    {
        _logs.Clear();
        _logs.AddRange(LogService.GetLogs());
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        // Unsubscribe from the OnLogAdded event to prevent memory leaks
        LogService.OnLogAdded -= UpdateLogs;
    }
}