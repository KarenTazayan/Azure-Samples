using Microsoft.AspNetCore.Components;

namespace AcsEmailEventsHandler.WebApp.Home;

public partial class HomePage
{
    [Inject] private HomePageViewModel ViewModel { get; init; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ViewModel.PropertyChanged += (_, _) => StateHasChanged();
        await ViewModel.InitializeAsync();
    }

    private async Task LoadData()
    {
        await ViewModel.LoadDataAsync();
    }
}