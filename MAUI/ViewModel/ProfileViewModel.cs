using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using MAUI.Helpers;
using MAUI.Services.Interfaces;
using MAUI.Views.Drawables;
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
        private readonly IFriendApiService _friendApiService;
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
            ISearchHistoryApiService searchHistoryApiService,
            IFriendApiService friendApiService)
            : base(sportsCache, cacheService, authAPIService, authService, playerApiService, teamApiService, leagueApiService, pushNotificationService, deviceTokenApiService, searchHistoryApiService)
        {
            _cacheService = cacheService;
            _userApiService = userApiService;
            _teamApiService = teamApiService;
            _predictionApiService = predictionApiService;
            _friendApiService = friendApiService;
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

        // Koliko redova se prikazuje dok korisnik ne klikne "Show more (N)". SVI podaci stizu jednim pozivom i
        // cuvaju se u memoriji (_all*), a ekran dobija samo prvih N - podataka je malo (desetine), a skup deo je
        // iscrtavanje redova sa slikama, ne prenos. Ovo je i razlog sto su "Show more"/"Show less" trenutni (bez novog poziva)
        // i sto filter prognoza radi lokalno.
        private const int ProfileListLimit = 5;

        private List<FavoriteTeamDTO> _allFavoriteTeams = new();

        // Namerno se NE kesira (za razliku od profila preko IUserCacheService) - lista se menja
        // sa tri razlicita mesta (srca na EventDetailsPage, dugme na TeamPage, kanta ovde), pa je
        // najjednostavnije i najsigurnije uvek uzeti svez podatak pri svakom otvaranju stranice.
        // Ovo je PRIKAZANI deo (prvih ProfileListLimit ili sve nakon "Show more").
        public ObservableCollection<FavoriteTeamDTO> FavoriteTeams { get; } = new();

        [ObservableProperty]
        private bool isFavoriteTeamsExpanded;

        public bool HasFavoriteTeams => _allFavoriteTeams.Count > 0;

        // Dugme postoji cim ima vise od ProfileListLimit stavki - "Show more (N)" (N = ukupno) dok je lista skracena,
        // "Show less" dok je prosirena.
        public bool HasMoreFavoriteTeams => _allFavoriteTeams.Count > ProfileListLimit;

        public string ToggleFavoriteTeamsText => IsFavoriteTeamsExpanded ? "Show less" : $"Show more ({_allFavoriteTeams.Count})";

        // Lista prognoza sa filterima i "Show more / Show less" - zajednicka sa UserPage (PredictionsListModel).
        public PredictionsListModel Predictions { get; } = new();

        // ---- Friends (SERVER paging: svaki "Show 10 more" salje novi zahtev, lista se ne ucitava cela) ----
        private const int FriendsInitialCount = 5;
        private const int FriendsPageSize = 10;

        public ObservableCollection<FriendDTO> Friends { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMoreFriends))]
        [NotifyPropertyChangedFor(nameof(ShowMoreFriendsText))]
        private int friendsTotal;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanShowMoreFriends))]
        private bool isFriendsLoading;

        // ---- "Search user": isti obrazac kao pretraga na pocetnoj stranici (debounce + minimum znakova), samo za
        // korisnicka imena. Rezultati se ispisuju odmah ispod polja (u toku stranice, ne kao preklapanje).
        [ObservableProperty]
        private string friendSearchText = string.Empty;

        public ObservableCollection<UserSearchResultDTO> UserSearchResults { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoUserResults))]
        private bool isUserSearchVisible;

        [ObservableProperty]
        private bool isUserSearching;

        public bool HasNoUserResults => IsUserSearchVisible && UserSearchResults.Count == 0;

        private CancellationTokenSource? _userSearchCts;

        partial void OnFriendSearchTextChanged(string value)
        {
            _userSearchCts?.Cancel();

            var trimmed = value?.Trim() ?? string.Empty;

            if (trimmed.Length < SearchMinQueryLength)
            {
                UserSearchResults.Clear();
                IsUserSearchVisible = false;
                IsUserSearching = false;
                return;
            }

            var cts = new CancellationTokenSource();
            _userSearchCts = cts;
            _ = RunUserSearchAsync(trimmed, cts.Token);
        }

        private async Task RunUserSearchAsync(string query, CancellationToken ct)
        {
            try
            {
                await Task.Delay(SearchDebounceDelay, ct);
                IsUserSearching = true;

                var results = await _userApiService.SearchUsersAsync(query, ct);
                if (ct.IsCancellationRequested)
                    return;

                Fill(UserSearchResults, results);
                IsUserSearchVisible = true;
                OnPropertyChanged(nameof(HasNoUserResults));
            }
            catch (OperationCanceledException)
            {
                // Korisnik je nastavio da kuca - ovaj upit je zastareo.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RunUserSearchAsync failed: {ex}");
            }
            finally
            {
                if (!ct.IsCancellationRequested)
                    IsUserSearching = false;
            }
        }

        // Klik na korisnika u rezultatima -> njegova UserPage; pretraga se prazni da se pri povratku ne vidi stara lista.
        [RelayCommand]
        private async Task UserSelected(long userId)
        {
            FriendSearchText = string.Empty;
            await Shell.Current.GoToAsync($"userpage?userId={userId}");
        }

        // ---- Dolazni zahtevi za prijateljstvo ("Friend requests (n)") - Accept (kvacica) / Decline (x) ----
        // Server vraca SVE zahteve odjednom (lista je kratka); ekran dobija prvih 5, ostatak na "Show more (N)" / "Show less".
        private const int RequestsLimit = 5;

        private List<FriendRequestDTO> _allFriendRequests = new();

        public ObservableCollection<FriendRequestDTO> FriendRequests { get; } = new();

        [ObservableProperty]
        private bool isFriendRequestsExpanded;

        public bool HasFriendRequests => _allFriendRequests.Count > 0;

        public string FriendRequestsTitle => $"Friend requests ({_allFriendRequests.Count})";

        public bool HasMoreFriendRequests => _allFriendRequests.Count > RequestsLimit;

        public string ToggleFriendRequestsText => IsFriendRequestsExpanded ? "Show less" : $"Show more ({_allFriendRequests.Count})";

        private async Task LoadFriendRequestsAsync()
        {
            try
            {
                _allFriendRequests = await _friendApiService.GetIncomingRequestsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFriendRequestsAsync failed: {ex}");
            }
            finally
            {
                ApplyFriendRequestsView();
            }
        }

        private void ApplyFriendRequestsView()
        {
            Fill(FriendRequests, IsFriendRequestsExpanded ? _allFriendRequests : _allFriendRequests.Take(RequestsLimit));

            OnPropertyChanged(nameof(HasFriendRequests));
            OnPropertyChanged(nameof(FriendRequestsTitle));
            OnPropertyChanged(nameof(HasMoreFriendRequests));
            OnPropertyChanged(nameof(ToggleFriendRequestsText));
        }

        partial void OnIsFriendRequestsExpandedChanged(bool value) => ApplyFriendRequestsView();

        [RelayCommand]
        private void ToggleFriendRequests() => IsFriendRequestsExpanded = !IsFriendRequestsExpanded;

        // X pored prijatelja - potvrda (modal), pa uklanjanje. Menja listu prijatelja, rang-listu i plasman.
        [RelayCommand]
        private async Task RemoveFriendAsync(FriendDTO friend)
        {
            if (friend is null)
                return;

            if (!await FriendDialogs.ConfirmRemoveAsync(friend.Username))
                return;

            try
            {
                await _friendApiService.RemoveFriendAsync(friend.UserId);
                await LoadFriendsAsync();
                await LoadStatsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to remove the friend.";
                System.Diagnostics.Debug.WriteLine($"RemoveFriendAsync failed: {ex}");
            }
        }

        [RelayCommand]
        private async Task AcceptRequestAsync(long userId)
        {
            try
            {
                await _friendApiService.AcceptRequestAsync(userId);

                // Novi prijatelj menja listu prijatelja, rang-listu i plasman - sve se osvezava.
                await LoadFriendRequestsAsync();
                await LoadFriendsAsync();
                await LoadStatsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to accept the request.";
                System.Diagnostics.Debug.WriteLine($"AcceptRequestAsync failed: {ex}");
            }
        }

        [RelayCommand]
        private async Task DeclineRequestAsync(long userId)
        {
            try
            {
                await _friendApiService.DeclineRequestAsync(userId);
                await LoadFriendRequestsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to decline the request.";
                System.Diagnostics.Debug.WriteLine($"DeclineRequestAsync failed: {ex}");
            }
        }

        // Klik na prijatelja -> njegova UserPage.
        [RelayCommand]
        private async Task FriendSelected(long userId)
        {
            await Shell.Current.GoToAsync($"userpage?userId={userId}");
        }

        public bool HasFriends => Friends.Count > 0;

        public bool HasNoFriends => Friends.Count == 0 && !IsFriendsLoading;

        public bool HasMoreFriends => Friends.Count < FriendsTotal;

        public bool CanShowMoreFriends => !IsFriendsLoading;

        public string ShowMoreFriendsText => $"Show {Math.Min(FriendsPageSize, Math.Max(FriendsTotal - Friends.Count, 0))} more";

        // Drugo dugme se pojavljuje tek kad je lista prosirena preko podrazumevanih 5.
        public bool CanCollapseFriends => Friends.Count > FriendsInitialCount;

        // ---- Statistika i rang-lista (kartica "Prediction stats" - tabovi Stats / Leaderboard) ----
        // Sve brojke (bodovi, ishodi, rang) racuna server (PredictionScoring); ovde se samo formatiraju za prikaz.
        [ObservableProperty]
        private PredictionStatsDTO? stats;

        [ObservableProperty]
        private IDrawable? donutDrawable;

        public ObservableCollection<FormDotDTO> FormDots { get; } = new();

        public bool HasStats => Stats is { TotalPredictions: > 0 };

        public bool HasNoStats => !HasStats;

        public bool HasSettledStats => Stats is { Settled: > 0 };

        public string PointsText => (Stats?.Points ?? 0).ToString();

        public string SuccessRateText => Stats is { Settled: > 0 } ? $"{Stats.SuccessRate:0.#}%" : "-";

        public string StreakText => Stats is null ? "-" : $"{Stats.CurrentStreak} (best {Stats.BestStreak})";

        // Rang ima smisla tek kad na listi ima jos nekog (prijatelja) osim mene.
        public string RankText => Stats is { Rank: int rank, RankedUsers: > 1 } ? $"#{rank} of {Stats.RankedUsers}" : "-";

        // Bodovi po nivou zavise od sporta (tabela SportScoringRules na serveru), pa legenda pokazuje samo BROJ pogodaka.
        public string ExactText => $"Exact  {Stats?.Exact ?? 0}";

        public string CloseText => $"Close  {Stats?.Close ?? 0}";

        public string WinnerText => $"Winner  {Stats?.Winner ?? 0}";

        public string MissedText => $"Missed  {Stats?.Missed ?? 0}";

        // Predvidjanja bez ishoda ("pending") i ponistena (otkazani mecevi) - van statistike, ali vidljiva korisniku.
        public string OtherStatsText
        {
            get
            {
                if (Stats is null)
                    return string.Empty;

                var parts = new List<string>();
                if (Stats.Pending > 0)
                    parts.Add($"{Stats.Pending} pending");
                if (Stats.Voided > 0)
                    parts.Add($"{Stats.Voided} cancelled (no points)");
                return string.Join(" · ", parts);
            }
        }

        public bool HasOtherStats => OtherStatsText.Length > 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStatsTab))]
        [NotifyPropertyChangedFor(nameof(IsLeaderboardTab))]
        [NotifyPropertyChangedFor(nameof(StatsTabColor))]
        [NotifyPropertyChangedFor(nameof(LeaderboardTabColor))]
        private string selectedStatsTab = StatsTabName;

        private const string StatsTabName = "Stats";
        private const string LeaderboardTabName = "Leaderboard";

        public bool IsStatsTab => SelectedStatsTab == StatsTabName;

        public bool IsLeaderboardTab => SelectedStatsTab == LeaderboardTabName;

        public Color StatsTabColor => IsStatsTab ? Color.FromArgb("#1A1A2E") : Color.FromArgb("#8A8A8A");

        public Color LeaderboardTabColor => IsLeaderboardTab ? Color.FromArgb("#1A1A2E") : Color.FromArgb("#8A8A8A");

        public ObservableCollection<LeaderboardRowDTO> LeaderboardRows { get; } = new();

        // Red ulogovanog korisnika ispod top liste - samo kad on NIJE medju prikazanima.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasLeaderboardMeRow))]
        [NotifyPropertyChangedFor(nameof(LeaderboardMeRows))]
        private LeaderboardRowDTO? leaderboardMeRow;

        public bool HasLeaderboardMeRow => LeaderboardMeRow is not null;

        // Jedan red (ili nijedan) - u obliku liste da XAML moze da ponovo iskoristi isti red-sablon kao top lista.
        public IEnumerable<LeaderboardRowDTO> LeaderboardMeRows => LeaderboardMeRow is null
            ? Array.Empty<LeaderboardRowDTO>()
            : new[] { LeaderboardMeRow };

        [ObservableProperty]
        private bool isLeaderboardLoading;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasLeaderboardNote))]
        private string leaderboardNote = string.Empty;

        public bool HasLeaderboardNote => LeaderboardNote.Length > 0;

        public bool HasLeaderboardRows => LeaderboardRows.Count > 0;
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
                await LoadFriendRequestsAsync();
                await LoadFriendsAsync();
                await LoadPredictionsAsync();
                await LoadStatsAsync();
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
                Predictions.SetItems(await _predictionApiService.GetMyPredictionsAsync());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadPredictionsAsync failed: {ex}");
            }
        }

        // Prvih FriendsInitialCount (poslednje dodati). Poziva se pri svakom otvaranju stranice - lista se resetuje.
        private async Task LoadFriendsAsync()
        {
            IsFriendsLoading = true;
            try
            {
                var page = await _friendApiService.GetFriendsAsync(0, FriendsInitialCount);
                Fill(Friends, page.Items);
                FriendsTotal = page.Total;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFriendsAsync failed: {ex}");
            }
            finally
            {
                IsFriendsLoading = false;
                NotifyFriendsChanged();
            }
        }

        // Svaki klik = NOV zahtev serveru za sledecih FriendsPageSize (skip = koliko je vec prikazano).
        [RelayCommand]
        private async Task ShowMoreFriendsAsync()
        {
            if (IsFriendsLoading || !HasMoreFriends)
                return;

            IsFriendsLoading = true;
            try
            {
                var page = await _friendApiService.GetFriendsAsync(Friends.Count, FriendsPageSize);

                // Ako je u medjuvremenu dodat nov prijatelj, ostali se pomeraju - preskace se vec prikazan.
                var known = Friends.Select(f => f.UserId).ToHashSet();
                foreach (var friend in page.Items.Where(f => !known.Contains(f.UserId)))
                    Friends.Add(friend);

                FriendsTotal = page.Total;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowMoreFriendsAsync failed: {ex}");
            }
            finally
            {
                IsFriendsLoading = false;
                NotifyFriendsChanged();
            }
        }

        // Nazad na podrazumevanih prvih 5 - bez novog poziva (to su vec ucitani, najnoviji prijatelji).
        [RelayCommand]
        private void CollapseFriends()
        {
            while (Friends.Count > FriendsInitialCount)
                Friends.RemoveAt(Friends.Count - 1);

            NotifyFriendsChanged();
        }

        private void NotifyFriendsChanged()
        {
            OnPropertyChanged(nameof(HasFriends));
            OnPropertyChanged(nameof(HasNoFriends));
            OnPropertyChanged(nameof(HasMoreFriends));
            OnPropertyChanged(nameof(ShowMoreFriendsText));
            OnPropertyChanged(nameof(CanCollapseFriends));
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                Stats = await _predictionApiService.GetMyStatsAsync();

                // Rang-lista se osvezava zajedno sa profilom ako je upravo ona izabrana (inace tek kad se tab otvori).
                if (IsLeaderboardTab)
                    await LoadLeaderboardAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadStatsAsync failed: {ex}");
            }
        }

        partial void OnStatsChanged(PredictionStatsDTO? value)
        {
            DonutDrawable = new PredictionDonutDrawable(value?.Exact ?? 0, value?.Close ?? 0, value?.Winner ?? 0, value?.Missed ?? 0, value?.Points ?? 0);

            Fill(FormDots, (value?.RecentForm ?? new List<string>()).Select(letter => new FormDotDTO
            {
                Letter = letter,
                Color = letter switch
                {
                    "E" => PredictionColors.Exact,
                    "C" => PredictionColors.Close,
                    "W" => PredictionColors.Winner,
                    _ => PredictionColors.Missed
                }
            }));

            foreach (var name in new[]
            {
                nameof(HasStats), nameof(HasNoStats), nameof(HasSettledStats), nameof(PointsText), nameof(SuccessRateText),
                nameof(StreakText), nameof(RankText), nameof(ExactText), nameof(CloseText), nameof(WinnerText), nameof(MissedText),
                nameof(OtherStatsText), nameof(HasOtherStats)
            })
                OnPropertyChanged(name);
        }

        [RelayCommand]
        private async Task SelectStatsTabAsync(string tab)
        {
            SelectedStatsTab = tab == LeaderboardTabName ? LeaderboardTabName : StatsTabName;

            if (IsLeaderboardTab)
                await LoadLeaderboardAsync();
        }

        private async Task LoadLeaderboardAsync()
        {
            IsLeaderboardLoading = true;
            try
            {
                var board = await _predictionApiService.GetLeaderboardAsync();

                var top = board.Entries.Select(e => e.Points).DefaultIfEmpty(0).Max();
                if (board.Me is not null)
                    top = Math.Max(top, board.Me.Points);

                LeaderboardRowDTO ToRow(LeaderboardEntryDTO e) => new()
                {
                    UserId = e.UserId,
                    Rank = e.Rank,
                    Username = e.Username,
                    Points = e.Points,
                    Exact = e.Exact,
                    Settled = e.Settled,
                    IsMe = e.IsMe,
                    Ratio = top == 0 ? 0 : (double)e.Points / top
                };

                Fill(LeaderboardRows, board.Entries.Select(ToRow));
                OnPropertyChanged(nameof(HasLeaderboardRows));

                LeaderboardMeRow = board.Me is not null && !board.Entries.Any(e => e.IsMe) ? ToRow(board.Me) : null;

                // Rang-lista = ja + prijatelji; dok nema prijatelja, na listi sam samo ja.
                LeaderboardNote = board.Entries.Count <= 1 ? "Add friends to compare your predictions with theirs." : string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadLeaderboardAsync failed: {ex}");
                LeaderboardNote = "Failed to load the leaderboard.";
            }
            finally
            {
                IsLeaderboardLoading = false;
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
            OnPropertyChanged(nameof(ToggleFavoriteTeamsText));
        }

        [RelayCommand]
        private void ToggleFavoriteTeams() => IsFavoriteTeamsExpanded = !IsFavoriteTeamsExpanded;

        partial void OnIsFavoriteTeamsExpandedChanged(bool value) => ApplyFavoriteTeamsView();

        [RelayCommand]
        private async Task FavoriteTeamSelected(long teamId)
        {
            await Shell.Current.GoToAsync($"teampage?teamId={teamId}");
        }

        [RelayCommand]
        private async Task RemoveFavoriteTeamAsync(FavoriteTeamDTO team)
        {
            if (team is null)
                return;

            if (!await ConfirmDialog.ShowAsync(
                    "Delete this team from favorite teams?",
                    "When you remove this team from favorite teams list, you will not receive notifications about this team until you add it again into favorites."))
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
