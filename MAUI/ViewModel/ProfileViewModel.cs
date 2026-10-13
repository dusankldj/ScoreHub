using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MAUI.ViewModel
{
    public partial class ProfileViewModel : NavMenuViewModelBase
    {
        #region Fields
        private readonly IUserCacheService _cacheService;
        private readonly IUserApiService _userApiService;
        private readonly ITeamApiService _teamApiService;
        private readonly IPredictionApiService _predictionApiService;
        #endregion

        #region Constructor
        public ProfileViewModel(
            ISportCacheService sportsCache,
            IAuthApiService authAPIService,
            IAuthService authService,
            IUserCacheService cacheService,
            IUserApiService userApiService,
            IPlayerApiService playerApiService,
            ITeamApiService teamApiService,
            ILeagueApiService leagueApiService,
            IPushNotificationService pushNotificationService,
            IDeviceTokenApiService deviceTokenApiService,
            IPredictionApiService predictionApiService,
            ISearchHistoryApiService searchHistoryApiService)
            : base(sportsCache, cacheService, authAPIService, authService, playerApiService, teamApiService, leagueApiService, pushNotificationService, deviceTokenApiService, searchHistoryApiService)
        {
            _cacheService = cacheService;
            _userApiService = userApiService;
            _teamApiService = teamApiService;
            _predictionApiService = predictionApiService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        private string firstName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string memberSince = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        // Koliko redova se prikazuje dok korisnik ne klikne "Show all (N)". SVI podaci stizu jednim pozivom i
        // cuvaju se u memoriji (_all*), a ekran dobija samo prvih N - podataka je malo (desetine), a skup deo je
        // iscrtavanje redova sa slikama, ne prenos. Ovo je i razlog sto je "Show all" trenutan (bez novog poziva)
        // i sto filter prognoza radi lokalno.
        private const int ProfileListLimit = 10;

        private List<FavoriteTeamDTO> _allFavoriteTeams = new();
        private List<MyPredictionDTO> _allPredictions = new();
        private int _filteredPredictionCount;

        // Namerno se NE kesira (za razliku od profila preko IUserCacheService) - lista se menja
        // sa tri razlicita mesta (srca na EventDetailsPage, dugme na TeamPage, kanta ovde), pa je
        // najjednostavnije i najsigurnije uvek uzeti svez podatak pri svakom otvaranju stranice.
        // Ovo je PRIKAZANI deo (prvih ProfileListLimit ili sve nakon "Show all").
        public ObservableCollection<FavoriteTeamDTO> FavoriteTeams { get; } = new();

        [ObservableProperty]
        private bool isFavoriteTeamsExpanded;

        public bool HasFavoriteTeams => _allFavoriteTeams.Count > 0;

        public bool HasMoreFavoriteTeams => !IsFavoriteTeamsExpanded && _allFavoriteTeams.Count > ProfileListLimit;

        public string ShowAllFavoriteTeamsText => $"Show all ({_allFavoriteTeams.Count})";

        // "My predictions" - tri odvojene liste (isti izvor, podeljen po stanju meca) da svaka
        // sekcija ima svoj naslov i sakrije se kad je prazna. Takodje se ne kesira - uvek svez podatak.
        public ObservableCollection<MyPredictionDTO> FinishedPredictions { get; } = new();

        public ObservableCollection<MyPredictionDTO> LivePredictions { get; } = new();

        public ObservableCollection<MyPredictionDTO> UpcomingPredictions { get; } = new();

        public bool HasFinishedPredictions => FinishedPredictions.Count > 0;

        public bool HasLivePredictions => LivePredictions.Count > 0;

        public bool HasUpcomingPredictions => UpcomingPredictions.Count > 0;

        // Naslov i filter se vide cim korisnik IKAD ima prognozu (ne zavisi od izabranog filtera) - inace
        // bi filter sa praznim rezultatom nestao zajedno sa sekcijom i ne bi se moglo vratiti na "All".
        public bool HasAnyPredictions => _allPredictions.Count > 0;

        public IReadOnlyList<string> PredictionFilterOptions { get; } = new[] { "All", "Finished", "Live", "Upcoming" };

        [ObservableProperty]
        private string selectedPredictionFilter = "All";

        // Drugi filter - ishod prognoze. "Hit"/"Missed" postoje samo za ZAVRSENE mecove (Hit je null dok mec
        // traje ili nije poceo), pa se u kombinaciji sa Live/Upcoming statusom dobija "No predictions".
        public IReadOnlyList<string> ResultFilterOptions { get; } = new[] { "All", "Hit", "Missed" };

        [ObservableProperty]
        private string selectedResultFilter = "All";

        [ObservableProperty]
        private bool isPredictionsExpanded;

        public bool HasNoPredictionsForFilter => _allPredictions.Count > 0 && _filteredPredictionCount == 0;

        public bool HasMorePredictions => !IsPredictionsExpanded && _filteredPredictionCount > ProfileListLimit;

        public string ShowAllPredictionsText => $"Show all ({_filteredPredictionCount})";
        #endregion

        #region Methods
        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var profile = await _cacheService.GetMeAsync();
                FirstName = profile.FirstName;
                LastName = profile.LastName;
                Username = "Username: " + profile.Username;
                MemberSince = profile.CreatedAt.ToString("dd/MM/yyyy");

                await LoadFavoriteTeamsAsync();
                await LoadPredictionsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load profile.";
                System.Diagnostics.Debug.WriteLine($"LoadProfileAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadFavoriteTeamsAsync()
        {
            try
            {
                _allFavoriteTeams = (await _userApiService.GetFavoriteTeamsAsync()).ToList();
                ApplyFavoriteTeamsView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFavoriteTeamsAsync failed: {ex}");
            }
        }

        private async Task LoadPredictionsAsync()
        {
            try
            {
                _allPredictions = await _predictionApiService.GetMyPredictionsAsync();
                ApplyPredictionsView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadPredictionsAsync failed: {ex}");
            }
        }

        private static void Fill<T>(ObservableCollection<T> target, IEnumerable<T> items)
        {
            target.Clear();
            foreach (var item in items)
                target.Add(item);
        }

        private void ApplyFavoriteTeamsView()
        {
            Fill(FavoriteTeams, IsFavoriteTeamsExpanded ? _allFavoriteTeams : _allFavoriteTeams.Take(ProfileListLimit));

            OnPropertyChanged(nameof(HasFavoriteTeams));
            OnPropertyChanged(nameof(HasMoreFavoriteTeams));
            OnPropertyChanged(nameof(ShowAllFavoriteTeamsText));
        }

        // Filter -> sortiranje po sekcijama -> UKUPNO ogranicenje na ProfileListLimit (redom Finished, Live,
        // Upcoming) -> popunjavanje tri liste. "Show all (N)" i "No predictions" se racunaju nad FILTRIRANIM
        // skupom. Napomena: u "All" prikazu, ko ima 10+ zavrsenih vidi samo njih dok ne klikne "Show all" -
        // za Live/Upcoming sluzi filter.
        private void ApplyPredictionsView()
        {
            IEnumerable<MyPredictionDTO> filtered = SelectedPredictionFilter switch
            {
                "Finished" => _allPredictions.Where(p => p.IsFinished),
                "Live" => _allPredictions.Where(p => p.IsLive),
                "Upcoming" => _allPredictions.Where(p => p.IsUpcoming),
                _ => _allPredictions
            };

            filtered = SelectedResultFilter switch
            {
                "Hit" => filtered.Where(p => p.Hit == true),
                "Missed" => filtered.Where(p => p.Hit == false),
                _ => filtered
            };

            var finished = filtered.Where(p => p.IsFinished).OrderByDescending(p => p.EventDate).ToList();
            var live = filtered.Where(p => p.IsLive).OrderBy(p => p.EventDate).ToList();
            var upcoming = filtered.Where(p => p.IsUpcoming).OrderBy(p => p.EventDate).ToList();

            _filteredPredictionCount = finished.Count + live.Count + upcoming.Count;

            var remaining = IsPredictionsExpanded ? int.MaxValue : ProfileListLimit;

            var shownFinished = finished.Take(remaining).ToList();
            remaining -= shownFinished.Count;
            var shownLive = live.Take(remaining).ToList();
            remaining -= shownLive.Count;
            var shownUpcoming = upcoming.Take(remaining).ToList();

            Fill(FinishedPredictions, shownFinished);
            Fill(LivePredictions, shownLive);
            Fill(UpcomingPredictions, shownUpcoming);

            OnPropertyChanged(nameof(HasFinishedPredictions));
            OnPropertyChanged(nameof(HasLivePredictions));
            OnPropertyChanged(nameof(HasUpcomingPredictions));
            OnPropertyChanged(nameof(HasAnyPredictions));
            OnPropertyChanged(nameof(HasNoPredictionsForFilter));
            OnPropertyChanged(nameof(HasMorePredictions));
            OnPropertyChanged(nameof(ShowAllPredictionsText));
        }

        [RelayCommand]
        private void ShowAllFavoriteTeams() => IsFavoriteTeamsExpanded = true;

        [RelayCommand]
        private void ShowAllPredictions() => IsPredictionsExpanded = true;

        partial void OnIsFavoriteTeamsExpandedChanged(bool value) => ApplyFavoriteTeamsView();

        partial void OnIsPredictionsExpandedChanged(bool value) => ApplyPredictionsView();

        // Novi filter = nov skup, pa "Show all" kreće ispocetka (prvih 10 filtriranih).
        partial void OnSelectedPredictionFilterChanged(string value) => RefreshAfterPredictionFilterChange();

        partial void OnSelectedResultFilterChanged(string value) => RefreshAfterPredictionFilterChange();

        private void RefreshAfterPredictionFilterChange()
        {
            if (IsPredictionsExpanded)
                IsPredictionsExpanded = false; // okida ApplyPredictionsView preko OnIsPredictionsExpandedChanged
            else
                ApplyPredictionsView();
        }

        [RelayCommand]
        private async Task PredictionSelected(long eventId)
        {
            await Shell.Current.GoToAsync($"eventdetails?eventId={eventId}");
        }

        [RelayCommand]
        private async Task RemoveFavoriteTeamAsync(FavoriteTeamDTO team)
        {
            if (team is null)
                return;

            try
            {
                await _teamApiService.RemoveFavoriteTeamAsync(team.TeamId);
                await LoadFavoriteTeamsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to remove favorite team.";
                System.Diagnostics.Debug.WriteLine($"RemoveFavoriteTeamAsync failed: {ex}");
            }
        }
        #endregion
    }
}
