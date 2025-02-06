using CSharpFunctionalExtensions;
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

    private async Task<GridData<EmailEvent>> ServerData(GridState<EmailEvent> gridState)
    {
        await ViewModel.LoadDataAsync(gridState.Page, gridState.PageSize);

        return new GridData<EmailEvent>
        {
            Items = ViewModel.AllEmailEvents,
            TotalItems = ViewModel.TotalRowsCount
        }; ;
    }
}