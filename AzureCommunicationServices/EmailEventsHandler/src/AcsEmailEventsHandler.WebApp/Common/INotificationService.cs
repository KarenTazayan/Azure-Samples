using MudBlazor;

namespace AcsEmailEventsHandler.WebApp.Common
{
    public interface INotificationService
    {
        public void Show(string message);
    }

    public class NotificationService(ISnackbar snackbar) : INotificationService
    {
        public void Show(string message)
        {
            snackbar.Add(message, Severity.Info);
        }
    }
}
