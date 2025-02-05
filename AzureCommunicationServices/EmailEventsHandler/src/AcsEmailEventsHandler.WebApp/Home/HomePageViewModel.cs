using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AcsEmailEventsHandler.WebApp.Home;

public class HomePageViewModel(EmailEventsRepository emailEventsRepository) : INotifyPropertyChanged
{
    public string Status { get; private set; } = "Initializing...Please reload the page after completion.";

    public readonly List<EmailEvent> AllEmailEvents = [];

    private bool _isLoadingInProgress = false;

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task LoadDataAsync()
    {
        if(_isLoadingInProgress) return;
        _isLoadingInProgress = !_isLoadingInProgress;

        Status = "Loading events please wait...";
        AllEmailEvents.Clear();
        var emailEvents = emailEventsRepository.GetEvents();
        await foreach (var emailEvent in emailEvents)
        {
            AllEmailEvents.Add(emailEvent);
            OnPropertyChanged(nameof(AllEmailEvents));
        }

        Status = "Events loaded successfully.";
        _isLoadingInProgress = !_isLoadingInProgress;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}