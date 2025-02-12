using AcsEmailEventsHandler.WebApp.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AcsEmailEventsHandler.WebApp.Home;

public class HomePageViewModel(EmailEventsRepository emailEventsRepository, 
    INotificationService notificationService) : INotifyPropertyChanged
{
    public readonly List<EmailEvent> AllEmailEvents = [];

    public int TotalRowsCount { get; private set; }

    private bool _isLoadingInProgress;

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task LoadDataAsync(DynamicSqlQuery dynamicSqlQuery, int pageNumber = 0, int pageSize = 15)
    {
        if(_isLoadingInProgress) return;
        _isLoadingInProgress = !_isLoadingInProgress;

        notificationService.Show("Loading events please wait...");

        var result = await emailEventsRepository.QueryEmailEvents(dynamicSqlQuery, pageNumber, pageSize);

        if (result.IsFailure)
        {
            notificationService.Show("Failed to load events.");
            _isLoadingInProgress = !_isLoadingInProgress;
            return;
        }

        TotalRowsCount = result.Value.TotalCount;
        OnPropertyChanged();
        AllEmailEvents.Clear();
        AllEmailEvents.AddRange(result.Value.Items);

        notificationService.Show("Events loaded successfully.");
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