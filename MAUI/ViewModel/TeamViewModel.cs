using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using MAUI.Services.Interfaces;

namespace MAUI.ViewModel
{
    [QueryProperty(nameof(TeamId), "teamId")]
    public partial class TeamViewModel : ObservableObject
    {
        #region Fields
        private readonly ITeamApiService _teamApiService;
        private readonly IAuthService _authService;
        #endregion

        #region Constructor
        public TeamViewModel(ITeamApiService teamApiService, IAuthService authService)
        {
            _teamApiService = teamApiService;
            _authService = authService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private long teamId;

        [ObservableProperty]
        private TeamDTO? details;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public bool HasStadium => !string.IsNullOrWhiteSpace(Details?.Stadium);

        public bool HasSportName => !string.IsNullOrWhiteSpace(Details?.SportName);

        public bool HasMatches => Details?.Matches?.Count > 0;

        public bool HasSquad => Details?.Squad?.Count > 0;

        public bool HasEquipment => Details?.Equipment?.Count > 0;

        public bool HasAnyTabData => HasMatches || HasSquad || HasEquipment;

        // Podrazumevano Matches - ako mec... tj. tim nema mecove, pada na prvi tab koji ima
        // podataka (Team members pa Team equipment).
        [ObservableProperty]
        private string selectedTab = "Matches";

        public bool IsMatchesSelected => SelectedTab == "Matches";

        public bool IsSquadSelected => SelectedTab == "Squad";

        public bool IsEquipmentSelected => SelectedTab == "Equipment";

        // Pozicija CarouselView-a - postavlja se na POSLEDNJU odigranu utakmicu kad se detalji
        // ucitaju (vidi OnDetailsChanged), a onda TwoWay binding prati sta korisnik odswajpuje.
        [ObservableProperty]
        private int selectedMatchIndex;

        [ObservableProperty]
        private bool isFavorited;

        public string FavoriteButtonText => IsFavorited ? "Remove from favorite teams" : "Add to favorite teams";

        public string FavoriteButtonIcon => IsFavorited ? Helpers.Icons.FullHearthIcon : Helpers.Icons.EmptyHearthIcon;

        public string FavoriteButtonIconFontFamily => IsFavorited ? "FA7Solid" : "FA7Regular";
        #endregion

        #region Methods
        partial void OnTeamIdChanged(long value)
        {
            _ = LoadTeamDetailsCommand.ExecuteAsync(null);
        }

        partial void OnDetailsChanged(TeamDTO? value)
        {
            OnPropertyChanged(nameof(HasStadium));
            OnPropertyChanged(nameof(HasSportName));
            OnPropertyChanged(nameof(HasMatches));
            OnPropertyChanged(nameof(HasSquad));
            OnPropertyChanged(nameof(HasEquipment));
            OnPropertyChanged(nameof(HasAnyTabData));

            SelectedTab = HasMatches ? "Matches" : HasSquad ? "Squad" : "Equipment";

            var matches = Details?.Matches;
            if (matches is { Count: > 0 })
            {
                var lastPlayedIndex = matches.FindLastIndex(m => m.IsPlayed);
                SelectedMatchIndex = lastPlayedIndex >= 0 ? lastPlayedIndex : 0;
            }

            _ = LoadFavoriteStatusAsync();
        }

        // Preskace se za neulogovanog korisnika - nema smisla gadjati [Authorize] endpoint.
        private async Task LoadFavoriteStatusAsync()
        {
            if (Details is null || !await _authService.IsLoggedInAsync())
                return;

            try
            {
                IsFavorited = await _teamApiService.IsTeamFavoritedAsync(TeamId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFavoriteStatusAsync failed: {ex}");
            }
        }

        partial void OnSelectedTabChanged(string value)
        {
            OnPropertyChanged(nameof(IsMatchesSelected));
            OnPropertyChanged(nameof(IsSquadSelected));
            OnPropertyChanged(nameof(IsEquipmentSelected));
        }

        partial void OnIsFavoritedChanged(bool value)
        {
            OnPropertyChanged(nameof(FavoriteButtonText));
            OnPropertyChanged(nameof(FavoriteButtonIcon));
            OnPropertyChanged(nameof(FavoriteButtonIconFontFamily));
        }

        [RelayCommand]
        private async Task LoadTeamDetailsAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                Details = await _teamApiService.GetTeamDetailsAsync(TeamId);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load team details.";
                System.Diagnostics.Debug.WriteLine($"LoadTeamDetailsAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
