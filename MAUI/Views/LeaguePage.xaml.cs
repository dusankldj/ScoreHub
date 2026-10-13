using MAUI.DTO.Input;
using MAUI.ViewModel;

namespace MAUI.Views;

public partial class LeaguePage : ContentPage
{
    public LeaguePage(LeagueViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnStandingRowTapped(object? sender, EventArgs e)
    {
        if (sender is Grid { BindingContext: LeagueStandingDTO standing })
            await Shell.Current.GoToAsync($"teampage?teamId={standing.TeamId}");
    }
}
