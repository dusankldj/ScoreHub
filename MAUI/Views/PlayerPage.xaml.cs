using MAUI.ViewModel;

namespace MAUI.Views;

public partial class PlayerPage : ContentPage
{
    private readonly PlayerViewModel _viewModel;

    public PlayerPage(PlayerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private async void OnStatisticsTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Statistics");

    private async void OnHonoursTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Honours");

    private async void OnMilestonesTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Milestones");

    private async void OnFormerTeamsTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("FormerTeams");

    private async void OnSocialsTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Socials");

    private async Task SwitchTabAsync(string tab)
    {
        if (_viewModel.SelectedTab == tab)
            return;

        await TabContentContainer.FadeTo(0, 120);
        _viewModel.SelectedTab = tab;
        await TabContentContainer.FadeTo(1, 150);
    }
}
