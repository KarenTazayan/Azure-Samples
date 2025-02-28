using AcsEmailEventsHandler.WebApp.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AcsEmailEventsHandler.WebApp.Home;

public partial class HomePage
{
    [Inject] private HomePageViewModel ViewModel { get; init; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ViewModel.PropertyChanged += (_, _) => StateHasChanged();
        await ViewModel.InitializeAsync();
    }

    private void OpenEventPayloadFilterDialog()
    {
        ViewModel.IsEventPayloadFilterVisible = true;
    }

    private void CloseEventPayloadFilterDialog()
    {
        ViewModel.IsEventPayloadFilterVisible = false;
    }

    private async Task ApplyEventPayloadFilter()
    {
        await ViewModel.ParseNlQueryToSqlAsync(ViewModel.EventPayloadFilterValue);
    }

    private async Task<GridData<EmailEvent>> ServerData(GridState<EmailEvent> gridState)
    {
        var dynamicSqlQuery = GridStateQueryParser.ParseGridStateToSql(gridState);

        await ViewModel.LoadDataAsync(dynamicSqlQuery, gridState.Page, gridState.PageSize);

        return new GridData<EmailEvent>
        {
            Items = ViewModel.AllEmailEvents,
            TotalItems = ViewModel.TotalRowsCount
        };
    }
}