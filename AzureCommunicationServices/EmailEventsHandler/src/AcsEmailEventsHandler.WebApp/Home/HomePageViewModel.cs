using AcsEmailEventsHandler.WebApp.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AcsEmailEventsHandler.WebApp.Home;

public class HomePageViewModel(EmailEventsRepository emailEventsRepository, NlQueryParser nlQueryParser,
    INotificationService notificationService) : INotifyPropertyChanged
{
    public readonly List<EmailEvent> AllEmailEvents = [];

    public readonly List<string> AiResponses = [];

    public bool IsAiFilterApplied { get; private set; }

    public int TotalRowsCount { get; private set; }

    private bool _isLoadingInProgress;

    public bool IsEventPayloadFilterVisible { get; set; }

    public string EventPayloadFilterValue { get; set; } = "Please show all events where recipient is [email]";

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
        AllEmailEvents.Clear();
        AllEmailEvents.AddRange(result.Value.Items);

        notificationService.Show("Events loaded successfully.");
        OnPropertyChanged();
        _isLoadingInProgress = !_isLoadingInProgress;
    }

    public async Task ParseNlQueryToSqlAsync(string nlQuery)
    {
        if (string.IsNullOrWhiteSpace(nlQuery))
        {
            notificationService.Show("Please provide a valid query.");
            return;
        }

        var result = await nlQueryParser.ParseNlToDynamicSqlQuery(nlQuery);

        if (result.IsSuccess)
        {
            notificationService.Show("Query parsed successfully.");
            AiResponses.Add(result.Value.ToString());
            IsAiFilterApplied = true;
            await LoadDataAsync(result.Value);
        }
        else
        {
            notificationService.Show("Failed to parse query.");
            notificationService.Show(result.Error);
        }

        OnPropertyChanged(nameof(AiResponses));
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