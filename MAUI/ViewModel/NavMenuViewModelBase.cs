using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.Helpers;
using MAUI.Services;
using MAUI.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MAUI.ViewModel
{
    public abstract partial class NavMenuViewModelBase : ObservableObject
    {
        #region Fields
        private readonly ISportCacheService _sportsCache;
        private readonly IUserCacheService _userCache;
        private readonly IAuthApiService _authAPIService;
        private readonly IAuthService _authService;
        private readonly IPlayerApiService _playerApiService;
        private readonly ITeamApiService _teamApiService;
        private readonly ILeagueApiService _leagueApiService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IDeviceTokenApiService _deviceTokenApiService;
        private readonly ISearchHistoryApiService _searchHistoryApiService;

        private const int SearchMinQueryLength = 2;

        private static readonly TimeSpan SearchDebounceDelay = TimeSpan.FromMilliseconds(750);

        private CancellationTokenSource? _searchDebounceCts;

        // Postavlja se SAMO dok SearchResultSelected programski prazni SearchText - bez toga bi prazan
        // tekst (dok polje jos ima fokus) odmah ponovo otvorio "Recent searches" usred navigacije.
        private bool _suppressRecents;
        #endregion

        #region Constructor
        protected NavMenuViewModelBase(
            ISportCacheService sportsCache,
            IUserCacheService userCache,
            IAuthApiService authAPIService,
            IAuthService authService,
            IPlayerApiService playerApiService,
            ITeamApiService teamApiService,
            ILeagueApiService leagueApiService,
            IPushNotificationService pushNotificationService,
            IDeviceTokenApiService deviceTokenApiService,
            ISearchHistoryApiService searchHistoryApiService)
        {
            _sportsCache = sportsCache;
            _userCache = userCache;
            _authAPIService = authAPIService;
            _authService = authService;
            _playerApiService = playerApiService;
            _teamApiService = teamApiService;
            _leagueApiService = leagueApiService;
            _pushNotificationService = pushNotificationService;
            _deviceTokenApiService = deviceTokenApiService;
            _searchHistoryApiService = searchHistoryApiService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private ObservableCollection<SportMenuItem> sports = new();

        [ObservableProperty]
        private ObservableCollection<SportMenuItem> overflowSports = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isLoggedIn;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<SearchResultItem> searchResults = new();

        [ObservableProperty]
        private bool isSearchResultsVisible;

        // Da li search polje trenutno ima fokus (dvosmerno vezano iz NavMenu) - fokus na PRAZNO polje
        // otvara "Recent searches".
        [ObservableProperty]
        private bool isSearchFocused;

        // Dropdown trenutno prikazuje "Recent searches" (a ne rezultate pretrage) - pali naslov sa "Clear all".
        [ObservableProperty]
        private bool isShowingRecents;

        public bool HasNoSearchResults => IsSearchResultsVisible && SearchResults.Count == 0;
        #endregion

        #region Methods
        [RelayCommand]
        private async Task LoadSportsAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                IsLoggedIn = await _authService.IsLoggedInAsync();

                var allSports = await _sportsCache.GetSportsAsync();

                Sports = new ObservableCollection<SportMenuItem>(
                    allSports.Take(4).Select(s =>
                    {
                        var icon = SportIconMap.GetIcon(s.Name);
                        return new SportMenuItem
                        {
                            Id = s.Id,
                            Name = s.Name,
                            IconGlyph = icon.Glyph,
                            IconFontFamily = icon.FontFamilyName
                        };
                    }));

                OverflowSports = new ObservableCollection<SportMenuItem>(
                    allSports.Skip(4).Select(s =>
                    {
                        var icon = SportIconMap.GetIcon(s.Name);
                        return new SportMenuItem
                        {
                            Id = s.Id,
                            Name = s.Name,
                            IconGlyph = icon.Glyph,
                            IconFontFamily = icon.FontFamilyName
                        };
                    }));
            }
            catch (Exception ex)
            {
                // Kratka poruka umesto ex.ToString() (ceo stack trace) - a Message obicno vec sadrzi
                // adresu kojoj se pristupalo, sto je najkorisnije kad je problem mrezna konfiguracija.
                ErrorMessage = $"Failed to load sports: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"LoadSportsAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        partial void OnSearchTextChanged(string value)
        {
            _searchDebounceCts?.Cancel();

            var trimmed = value?.Trim() ?? string.Empty;

            if (trimmed.Length < SearchMinQueryLength)
            {
                SearchResults = new ObservableCollection<SearchResultItem>();
                IsSearchResultsVisible = false;
                IsShowingRecents = false;

                // Korisnik je obrisao ukucani tekst dok polje jos ima fokus - ponovo ponudi "Recent searches".
                if (trimmed.Length == 0 && IsSearchFocused && !_suppressRecents)
                    _ = ShowRecentSearchesAsync();

                return;
            }

            IsShowingRecents = false;

            var cts = new CancellationTokenSource();
            _searchDebounceCts = cts;
            _ = RunDebouncedSearchAsync(trimmed, cts.Token);
        }

        partial void OnSearchResultsChanged(ObservableCollection<SearchResultItem> value)
        {
            OnPropertyChanged(nameof(HasNoSearchResults));
        }

        partial void OnIsSearchResultsVisibleChanged(bool value)
        {
            OnPropertyChanged(nameof(HasNoSearchResults));
        }

        private async Task RunDebouncedSearchAsync(string query, CancellationToken token)
        {
            try
            {
                await Task.Delay(SearchDebounceDelay, token);
                await ExecuteSearchAsync(query, token);
            }
            catch (OperationCanceledException)
            {
                //Korisnik je nastavio da kuca(ili obrisao tekst) pre isteka pauze 
            }
        }

        
        private async Task ExecuteSearchAsync(string query, CancellationToken token)
        {
            try
            {
                var playersTask = _playerApiService.SearchPlayersAsync(query, token);
                var teamsTask = _teamApiService.SearchTeamsAsync(query, token);
                var leaguesTask = _leagueApiService.SearchLeaguesAsync(query, token);

                await Task.WhenAll(playersTask, teamsTask, leaguesTask);

                if (token.IsCancellationRequested)
                    return;

                var results = new List<SearchResultItem>();

                results.AddRange(playersTask.Result.Select(p => new SearchResultItem
                {
                    Id = p.Id,
                    Kind = SearchResultKind.Player,
                    DisplayName = $"{p.FirstName} {p.LastName}".Trim(),
                    ImageUrl = p.ImageUrl,
                    SportName = p.SportName
                }));

                results.AddRange(teamsTask.Result.Select(t => new SearchResultItem
                {
                    Id = t.Id,
                    Kind = SearchResultKind.Team,
                    DisplayName = t.Name,
                    ImageUrl = t.ImageUrl
                }));

                results.AddRange(leaguesTask.Result.Select(l => new SearchResultItem
                {
                    Id = l.Id,
                    Kind = SearchResultKind.League,
                    DisplayName = l.Name,
                    ImageUrl = l.ImageUrl
                }));

                SearchResults = new ObservableCollection<SearchResultItem>(results);

                IsSearchResultsVisible = true;
            }
            catch (OperationCanceledException)
            {
                // Nova pretraga je vec u toku - ovaj (stari) poziv vise nije relevantan
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ExecuteSearchAsync failed: {ex}");
            }
        }

        
        partial void OnIsSearchFocusedChanged(bool value)
            => _ = HandleSearchFocusChangedAsync(value);

        private async Task HandleSearchFocusChangedAsync(bool focused)
        {
            if (focused)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    await ShowRecentSearchesAsync();

                return;
            }

            // Kratko kasnjenje - tap na red/X u dropdown-u ne sme da izgubi trku sa gubljenjem fokusa.
            await Task.Delay(200);

            if (!IsSearchFocused && IsShowingRecents)
                HideRecentSearches();
        }

        // Samo za ulogovanog korisnika. Dolazi vec sortirano (od poslednjeg ka prvom) sa servera.
        private async Task ShowRecentSearchesAsync()
        {
            if (!IsLoggedIn)
                return;

            try
            {
                var recents = await _searchHistoryApiService.GetRecentSearchesAsync();

                // Dok je odgovor stizao, korisnik je mozda poceo da kuca ili otisao sa polja - tada se odbacuje.
                if (!string.IsNullOrWhiteSpace(SearchText) || !IsSearchFocused)
                    return;

                var items = recents
                    .Where(r => Enum.TryParse<SearchResultKind>(r.Kind, ignoreCase: true, out _))
                    .Select(r => new SearchResultItem
                    {
                        Id = r.EntityId,
                        Kind = Enum.Parse<SearchResultKind>(r.Kind, ignoreCase: true),
                        DisplayName = r.DisplayName,
                        ImageUrl = r.ImageUrl,
                        SportName = r.SportName,
                        RecentId = r.Id
                    })
                    .ToList();

                if (items.Count == 0)
                    return;

                SearchResults = new ObservableCollection<SearchResultItem>(items);
                IsShowingRecents = true;
                IsSearchResultsVisible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowRecentSearchesAsync failed: {ex}");
            }
        }

        private void HideRecentSearches()
        {
            IsSearchResultsVisible = false;
            IsShowingRecents = false;
            SearchResults = new ObservableCollection<SearchResultItem>();
        }

        // X ikonica uz stavku - uklanja SAMO tu stavku. Prvo se uklanja lokalno (odmah nestaje sa ekrana),
        // pa se salje DELETE; ako on ne uspe, stavka se pojavi opet pri sledecem otvaranju liste.
        [RelayCommand]
        private async Task RemoveRecentSearch(SearchResultItem item)
        {
            if (item.RecentId is not long recentId)
                return;

            SearchResults.Remove(item);

            if (SearchResults.Count == 0)
                HideRecentSearches();

            try
            {
                await _searchHistoryApiService.RemoveSearchAsync(recentId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RemoveRecentSearch failed: {ex}");
            }
        }

        [RelayCommand]
        private async Task ClearRecentSearches()
        {
            HideRecentSearches();

            try
            {
                await _searchHistoryApiService.ClearSearchesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ClearRecentSearches failed: {ex}");
            }
        }

        // Fire-and-forget - neuspeh upisa u istoriju nikad ne sme da blokira ni uspori navigaciju.
        private async Task RecordSearchAsync(SearchResultItem result)
        {
            try
            {
                await _searchHistoryApiService.RecordSearchAsync(result.Kind.ToString(), result.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RecordSearchAsync failed: {ex}");
            }
        }

        [RelayCommand]
        private async Task SearchResultSelected(SearchResultItem result)
        {
            _suppressRecents = true;
            IsSearchResultsVisible = false;
            IsShowingRecents = false;
            SearchResults = new ObservableCollection<SearchResultItem>();
            SearchText = string.Empty;
            _suppressRecents = false;

            if (IsLoggedIn)
                _ = RecordSearchAsync(result);

            switch (result.Kind)
            {
                case SearchResultKind.Player:
                    await Shell.Current.GoToAsync($"playerpage?playerId={result.Id}");
                    break;
                case SearchResultKind.Team:
                    await Shell.Current.GoToAsync($"teampage?teamId={result.Id}");
                    break;
                case SearchResultKind.League:
                    await Shell.Current.GoToAsync($"leaguepage?leagueId={result.Id}");
                    break;
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            try
            {
                try
                {
                    await UnregisterDeviceTokenAsync();
                    await _authAPIService.LogoutAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Backend logout call failed (proceeding with local logout anyway): {ex}");
                }

                await _authService.LogoutAsync();
                _userCache.Clear();
                IsLoggedIn = false;
                await Shell.Current.GoToAsync("//home");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
                System.Diagnostics.Debug.WriteLine($"LogoutAsync failed: {ex}");
            }
        }

        
        private async Task UnregisterDeviceTokenAsync()
        {
            try
            {
                var token = await _pushNotificationService.GetTokenAsync();
                if (!string.IsNullOrWhiteSpace(token))
                    await _deviceTokenApiService.UnregisterTokenAsync(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UnregisterDeviceTokenAsync failed: {ex}");
            }
        }
        #endregion
    }
}
