using MAUI.DTO.Input;
using MAUI.Helpers;
using MAUI.Services.Interfaces;
using MAUI.ViewModel;

namespace MAUI.Views;

public partial class EventDetailsPage : ContentPage
{
    private readonly EventDetailsViewModel _viewModel;
    private readonly IAuthService _authService;
    private readonly ITeamApiService _teamApiService;

    public EventDetailsPage(EventDetailsViewModel viewModel, IAuthService authService, ITeamApiService teamApiService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _authService = authService;
        _teamApiService = teamApiService;
        BindingContext = viewModel;
    }

    private async void OnHomeFavoriteHeartTapped(object? sender, EventArgs e)
        => await ToggleFavoriteHeartAsync(sender, isHome: true);

    private async void OnAwayFavoriteHeartTapped(object? sender, EventArgs e)
        => await ToggleFavoriteHeartAsync(sender, isHome: false);

    // Prazno -> puno srce (i obrnuto) uz kratak "pop" - element skoci na 1.25x pa se vrati,
    // sama ikonica se menja NA VRHUNCU skoka (kad je najveca, promena je najmanje primetna).
    // Ako API poziv ne uspe, stanje se vraca nazad da UI ne bi lagao o tome sta je sacuvano.
    private async Task ToggleFavoriteHeartAsync(object? sender, bool isHome)
    {
        if (sender is not VisualElement element || _viewModel.Details is null)
            return;

        if (!await _authService.IsLoggedInAsync())
        {
            await Shell.Current.GoToAsync("login");
            return;
        }

        var teamId = isHome ? _viewModel.Details.HomeTeamId : _viewModel.Details.AwayTeamId;
        var newState = isHome ? !_viewModel.IsHomeTeamFavorited : !_viewModel.IsAwayTeamFavorited;

        await element.ScaleTo(1.25, 100, Easing.CubicOut);

        if (isHome)
            _viewModel.IsHomeTeamFavorited = newState;
        else
            _viewModel.IsAwayTeamFavorited = newState;

        await element.ScaleTo(1.0, 120, Easing.SpringOut);

        try
        {
            if (newState)
                await _teamApiService.AddFavoriteTeamAsync(teamId);
            else
                await _teamApiService.RemoveFavoriteTeamAsync(teamId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ToggleFavoriteHeartAsync failed: {ex}");
            if (isHome)
                _viewModel.IsHomeTeamFavorited = !newState;
            else
                _viewModel.IsAwayTeamFavorited = !newState;
        }
    }

    private async void OnLineupTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Lineup");

    private async void OnStatisticsTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Statistics");

    private async void OnBroadcastsTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Broadcasts");

    private async void OnFullTimelineTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("FullTimeline");

    private async void OnPlayerImageTapped(object? sender, EventArgs e)
    {
        if (sender is Image { BindingContext: EventLineupDTO player })
            await ImagePreviewHelper.ShowAsync(player.ImageUrl);
    }

    private async void OnPlayerRowTapped(object? sender, EventArgs e)
    {
        if (sender is VerticalStackLayout { BindingContext: EventLineupDTO player })
            await Shell.Current.GoToAsync($"playerpage?playerId={player.PlayerId}");
    }

    // Fade-out/fade-in umesto trenutne zamene sadrzaja - SelectedTab se menja DOK je
    // TabContentContainer providan, pa se novi sadrzaj "otkriva" fade-in-om.
    private async Task SwitchTabAsync(string tab)
    {
        if (_viewModel.SelectedTab == tab)
            return;

        await TabContentContainer.FadeTo(0, 120);
        _viewModel.SelectedTab = tab;
        await TabContentContainer.FadeTo(1, 150);
    }
}
