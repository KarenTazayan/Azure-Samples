using MudBlazor;

namespace AcsEmailEventsHandler.WebApp.Common;

public interface INotificationService
{
    public void Show(string message);
}

public class NotificationService(ISnackbar bar) : INotificationService
{
    public void Show(string message)
    {
        bar.Add(message, Severity.Info);
    }
}