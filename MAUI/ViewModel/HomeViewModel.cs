using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using MAUI.Services;
using MAUI.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace MAUI.ViewModel
{
    public partial class HomeViewModel : NavMenuViewModelBase
    {
        #region Fields
        private readonly IMatchesAPIService _matchesApiService;
        #endregion

        #region Constructor
        public HomeViewModel(
            ISportCacheService sportsCache,
            IUserCacheService userCache,
            IAuthApiService authAPIService,
            IAuthService authService,
            IMatchesAPIService matchesApiService,
            IPlayerApiService playerApiService,
            ITeamApiService teamApiService,
            ILeagueApiService leagueApiService,
            IPushNotificationService pushNotificationService,
            IDeviceTokenApiService deviceTokenApiService,
            ISearchHistoryApiService searchHistoryApiService)
        : base(sportsCache, userCache, authAPIService, authService, playerApiService, teamApiService, leagueApiService, pushNotificationService, deviceTokenApiService, searchHistoryApiService)
        {
            _matchesApiService = matchesApiService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private ObservableCollection<MatchesDTO> matches = new();

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private long? selectedSportId;

        [ObservableProperty]
        private string? selectedStatus;

        private CancellationTokenSource? _liveRefreshCts;

        private (DateTime date, long? sportId, string? status)? _loadedFilter;

        private bool _matchesErrorShown;

        // Spinner samo za "pravo" ucitavanje (prvo otvaranje, promena datuma/sporta/statusa, ili prazna lista).
        // IsBusy se NAMERNO ne koristi za spinner: postavlja se i pri tihom 2-minutnom live refresh-u (kad lista
        // vec stoji na ekranu), pa bi spinner treptao na svaka 2 minuta bez razloga.
        [ObservableProperty]
        private bool isInitialLoading;
        #endregion

        #region Methods
        partial void OnSelectedDateChanged(DateTime value)
        {
            _ = LoadMatchesCommand.ExecuteAsync(null);
        }

        partial void OnSelectedSportIdChanged(long? value)
        {
            _ = LoadMatchesCommand.ExecuteAsync(null);
        }

        partial void OnSelectedStatusChanged(string? value)
        {
            _ = LoadMatchesCommand.ExecuteAsync(null);
        }


        [RelayCommand]
        private void SportSelected(long sportId)
        {
            SelectedSportId = sportId;
        }

        [RelayCommand]
        private async Task EventSelected(long eventId)
        {
            await Shell.Current.GoToAsync($"eventdetails?eventId={eventId}");
        }

        [RelayCommand]
        private async Task LeagueSelected(long leagueId)
        {
            await Shell.Current.GoToAsync($"leaguepage?leagueId={leagueId}");
        }

        [RelayCommand]
        private async Task LoadMatchesAsync()
        {
            IsBusy = true;
            IsInitialLoading = _loadedFilter != (SelectedDate.Date, SelectedSportId, SelectedStatus) || Matches.Count == 0;

            // Brise se SAMO greska koju je postavilo prethodno ucitavanje meceva. Ranije se ErrorMessage
            // bezuslovno praznio, pa je greska iz LoadSportsAsync (koji se u OnAppearing izvrsava neposredno
            // pre ovog) nestajala u milisekundi cim krene ucitavanje meceva.
            if (_matchesErrorShown)
            {
                ErrorMessage = string.Empty;
                _matchesErrorShown = false;
            }

            try
            {
                var localMidnight = SelectedDate.Date;
                var fromUtc = localMidnight.ToUniversalTime();
                var toUtc = localMidnight.AddDays(1).ToUniversalTime();

                var result = await _matchesApiService.GetMatchesAsync(SelectedSportId, fromUtc, toUtc, SelectedStatus);


                var currentFilter = (localMidnight, SelectedSportId, SelectedStatus);
                bool filterChanged = _loadedFilter != currentFilter;
                if (filterChanged || Matches.Count == 0)
                {
                    Matches = new ObservableCollection<MatchesDTO>(result);
                    _loadedFilter = currentFilter;
                }
                else MergeMatches(result);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load matches: {ex.Message}";
                _matchesErrorShown = true;
                System.Diagnostics.Debug.WriteLine($"LoadMatchesAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
                IsInitialLoading = false;
            }

            if (HasLiveMatches())
                StartLiveRefreshIfNotRunning();
            else StopLiveRefresh();
        }

        private void MergeMatches(List<MatchesDTO> freshResult)
        {
            var freshEvents = freshResult
                .SelectMany(sport => sport.Leagues)
                .SelectMany(league => league.Events)
                .ToDictionary(e => e.EventId);

            for (int si = Matches.Count - 1; si >= 0; si--)
            {
                var sport = Matches[si];

                for (int li = sport.Leagues.Count - 1; li >= 0; li--)
                {
                    var league = sport.Leagues[li];

                    for (int ei = league.Events.Count - 1; ei >= 0; ei--)
                    {
                        var existing = league.Events[ei];

                        if (!freshEvents.TryGetValue(existing.EventId, out var fresh))
                        {
                            league.Events.RemoveAt(ei);
                            continue;
                        }

                        bool scoreChanged = existing.HomeTeamScore != fresh.HomeTeamScore
                            || existing.AwayTeamScore != fresh.AwayTeamScore;

                        existing.HomeTeamScore = fresh.HomeTeamScore;
                        existing.AwayTeamScore = fresh.AwayTeamScore;
                        existing.Status = fresh.Status;
                        existing.DetailedStatus = fresh.DetailedStatus;
                        existing.Progress = fresh.Progress;

                        if (scoreChanged)
                            _ = FlashScoreAsync(existing);
                    }

                    if (league.Events.Count == 0)
                        sport.Leagues.RemoveAt(li);
                }

                if (sport.Leagues.Count == 0)
                    Matches.RemoveAt(si);
            }
        }

        private static async Task FlashScoreAsync(EventsDTO ev)
        {
            ev.IsScoreFlashing = true;
            await Task.Delay(1000);
            ev.IsScoreFlashing = false;
        }

        private bool HasLiveMatches()
            => Matches.Any(sport => sport.Leagues?.Any(l => l.Events?.Any(e => e.Status == "Live") == true) == true);

        public void StartLiveRefreshIfNotRunning()
        {
            if (_liveRefreshCts is not null)
                return; 

            _liveRefreshCts = new CancellationTokenSource();
            _ = RunLiveRefreshLoopAsync(_liveRefreshCts.Token);
        }

        public void StopLiveRefresh()
        {
            _liveRefreshCts?.Cancel();
            _liveRefreshCts = null;
        }

        private async Task RunLiveRefreshLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(2), token);

                    if (token.IsCancellationRequested)
                        break;

                    await LoadMatchesCommand.ExecuteAsync(null);
                }
            }
            catch (OperationCanceledException)
            {
                // kada vise nema live meceva - nista ne radi.
            }
        }
        #endregion
    }
}
