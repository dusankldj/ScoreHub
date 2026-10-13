using MAUI.DTO.Input;
using MAUI.Helpers;
using MAUI.Services.Interfaces;
using MAUI.ViewModel;
using System.ComponentModel;

namespace MAUI.Views;

public partial class TeamPage : ContentPage
{
    private readonly TeamViewModel _viewModel;
    private readonly IAuthService _authService;
    private readonly ITeamApiService _teamApiService;

    public TeamPage(TeamViewModel viewModel, IAuthService authService, ITeamApiService teamApiService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _authService = authService;
        _teamApiService = teamApiService;
        BindingContext = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    // Isti obrazac kao srca na EventDetailsPage - login-gate, kratak "pop", pa tek onda
    // (na vrhuncu skoka) promena teksta/ikonice. Ako API poziv ne uspe, stanje se vraca nazad.
    private async void OnFavoriteButtonTapped(object? sender, EventArgs e)
    {
        if (sender is not VisualElement element)
            return;

        if (!await _authService.IsLoggedInAsync())
        {
            await Shell.Current.GoToAsync("login");
            return;
        }

        var newState = !_viewModel.IsFavorited;

        await element.ScaleTo(1.05, 100, Easing.CubicOut);
        _viewModel.IsFavorited = newState;
        await element.ScaleTo(1.0, 120, Easing.SpringOut);

        try
        {
            if (newState)
                await _teamApiService.AddFavoriteTeamAsync(_viewModel.TeamId);
            else
                await _teamApiService.RemoveFavoriteTeamAsync(_viewModel.TeamId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnFavoriteButtonTapped failed: {ex}");
            _viewModel.IsFavorited = !newState;
        }
    }

    // Namerno NEMA Position binding-a u XAML-u (vidi komentar tamo) - jedini nacin da se
    // carousel pozicionira je eksplicitni ScrollTo, ovde, DOK je ItemsSource vec postavljen
    // (CarouselView.ItemsSource se azurira PRE nego sto se PropertyChanged("Details") iz
    // ViewModel-a i pojavi, pa je SelectedMatchIndex promena - koja se desava JOS RANIJE,
    // unutar OnDetailsChanged - siguran okidac da vec postoje stavke za ScrollTo).
    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TeamViewModel.SelectedMatchIndex))
            return;

        var index = _viewModel.SelectedMatchIndex;

        for (var attempt = 1; attempt <= 6; attempt++)
        {
            await Task.Delay(100 * attempt);

            var matches = _viewModel.Details?.Matches;
            if (matches is not { Count: > 0 } || index < 0 || index >= matches.Count)
                continue;

            MatchesCarousel.ScrollTo(index, position: ScrollToPosition.Center, animate: false);

            if (MatchesCarousel.Position == index)
                return;
        }
    }

    private async void OnMatchesTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Matches");

    private async void OnSquadTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Squad");

    private async void OnEquipmentTabTapped(object? sender, EventArgs e)
        => await SwitchTabAsync("Equipment");

    private async void OnPlayerImageTapped(object? sender, EventArgs e)
    {
        if (sender is Image { BindingContext: Squad player })
            await ImagePreviewHelper.ShowAsync(player.ImageUrl);
    }

    private async void OnPlayerRowTapped(object? sender, EventArgs e)
    {
        if (sender is VerticalStackLayout { BindingContext: Squad player })
            await Shell.Current.GoToAsync($"playerpage?playerId={player.PlayerId}");
    }

    private async void OnMatchCardTapped(object? sender, EventArgs e)
    {
        if (sender is VerticalStackLayout { BindingContext: TeamMatches match })
            await Shell.Current.GoToAsync($"eventdetails?eventId={match.EventId}");
    }

    private async Task SwitchTabAsync(string tab)
    {
        if (_viewModel.SelectedTab == tab)
            return;

        await TabContentContainer.FadeTo(0, 120);
        _viewModel.SelectedTab = tab;
        await TabContentContainer.FadeTo(1, 150);
    }
}
