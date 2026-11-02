using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using MAUI.Services.Interfaces;

namespace MAUI.ViewModel
{
    [QueryProperty(nameof(EventId), "eventId")]
    public partial class EventDetailsViewModel : ObservableObject
    {
        #region Fields
        private readonly IEventDetailsApiService _eventDetailsApiService;
        private readonly ITeamApiService _teamApiService;
        private readonly IAuthService _authService;
        private readonly IPredictionApiService _predictionApiService;
        #endregion

        #region Constructor
        public EventDetailsViewModel(IEventDetailsApiService eventDetailsApiService, ITeamApiService teamApiService, IAuthService authService, IPredictionApiService predictionApiService)
        {
            _eventDetailsApiService = eventDetailsApiService;
            _teamApiService = teamApiService;
            _authService = authService;
            _predictionApiService = predictionApiService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private long eventId;

        [ObservableProperty]
        private EventDetailsDTO? details;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        // Skica trazi da se za "Upcoming" mecove umesto statusa u gornjem desnom uglu ispise
        // datum+vreme - lakse je ovo izracunati ovde nego u XAML-u preko vise nested Binding-a.
        public string HeaderRightText => Details is null
            ? string.Empty
            : Details.Status == "Upcoming"
                ? Details.LocalEventDate.ToString("dd/MM/yyyy HH:mm")
                : Details.Status ?? string.Empty;

        public bool IsLive => Details?.Status == "Live";

        // "1H"/"2H"/"HT" itd - samo dok je mec Live, ispod centriranog "Live" naslova.
        public bool ShowDetailedStatus => IsLive && !string.IsNullOrWhiteSpace(DetailedStatusDisplay);

        public bool HasProgress => !string.IsNullOrWhiteSpace(Details?.Progress);

        // "1H - 30'" (ili samo "1H" ako progres jos nije stigao) - JEDAN red umesto dva odvojena.
        // Ako provider jos javlja "NS" (ili nista) a minut vec tece, "NS" je zastareo - samo "2'".
        public string DetailedStatusDisplay
        {
            get
            {
                var status = Details?.DetailedStatus;

                if (HasProgress && (string.IsNullOrWhiteSpace(status) || status == "NS"))
                    return $"{Details?.Progress}'";

                return HasProgress ? $"{status} - {Details?.Progress}'" : status ?? string.Empty;
            }
        }

        public bool HasRound => !string.IsNullOrWhiteSpace(Details?.Round);

        public bool HasSeason => !string.IsNullOrWhiteSpace(Details?.Season);

        public bool HasLeagueName => !string.IsNullOrWhiteSpace(Details?.LeagueName);

        public bool HasVenue => !string.IsNullOrWhiteSpace(Details?.Venue);

        public bool HasLineups => Details?.Players?.Count > 0;

        public bool HasStatistics => Details?.Statistics?.Count > 0;

        public List<EventLineupDTO> HomeLineup => Details?.Players?.Where(p => p.HomeGame).ToList() ?? new();

        public List<EventLineupDTO> AwayLineup => Details?.Players?.Where(p => !p.HomeGame).ToList() ?? new();

        public bool HasBroadcasts => Details?.Broadcasts?.Count > 0;

        public bool HasTimeline => Details?.Timeline?.Count > 0;

        public bool HasFullTimeline => Details?.FullTimeline?.Count > 0;

        // Kanali po dva u redu (poslednji red ima samo Left ako je broj neparan).
        private List<EventBroadcastRowDTO> AllBroadcastRows => (Details?.Broadcasts ?? new())
            .Chunk(2)
            .Select(pair => new EventBroadcastRowDTO { Left = pair[0], Right = pair.Length > 1 ? pair[1] : null })
            .ToList();

        // Mec moze imati i 80+ kanala (svaki red = logo + zastavica, dve slike) - render svih odjednom
        // je uzrok "kocenja" pri prvom otvaranju/skrolovanju kartice. Prikazuje se prvih BroadcastRowLimit
        // redova, ostatak tek na "Show all" klik.
        private const int BroadcastRowLimit = 8;

        [ObservableProperty]
        private bool isBroadcastsExpanded;

        public List<EventBroadcastRowDTO> BroadcastRows => IsBroadcastsExpanded
            ? AllBroadcastRows
            : AllBroadcastRows.Take(BroadcastRowLimit).ToList();

        public bool HasMoreBroadcastRows => !IsBroadcastsExpanded && AllBroadcastRows.Count > BroadcastRowLimit;

        public string ShowAllBroadcastsText => $"Show all ({Details?.Broadcasts?.Count ?? 0})";

        [RelayCommand]
        private void ShowAllBroadcasts() => IsBroadcastsExpanded = true;

        partial void OnIsBroadcastsExpandedChanged(bool value)
        {
            OnPropertyChanged(nameof(BroadcastRows));
            OnPropertyChanged(nameof(HasMoreBroadcastRows));
        }

        public bool HasAnyDetailData => HasLineups || HasStatistics || HasBroadcasts || HasFullTimeline;

        // Podrazumevano Lineup - ako mec nema postavu pada na Statistics, pa Broadcasts, pa Full timeline.
        [ObservableProperty]
        private string selectedTab = "Lineup";

        public bool IsLineupSelected => SelectedTab == "Lineup";

        public bool IsStatisticsSelected => SelectedTab == "Statistics";

        public bool IsBroadcastsSelected => SelectedTab == "Broadcasts";

        public bool IsFullTimelineSelected => SelectedTab == "FullTimeline";

        // "Red Cards" je jedan od Stat redova u Statistics - ako tim ima bar jedan, prikazi
        // ikonicu kartona pored njegovog imena u header-u.
        public bool HasHomeRedCard => Details?.Statistics?.FirstOrDefault(s => s.Stat == "Red Cards")?.HomeScore > 0;

        public bool HasAwayRedCard => Details?.Statistics?.FirstOrDefault(s => s.Stat == "Red Cards")?.AwayScore > 0;

        // SAMO lokalno/sesijsko stanje za sada (nista se ne cuva) - login-gate i animacija su
        // gotovi, stvarno pamcenje omiljenih timova (FavoriteTeam tabela) dolazi kasnije.
        [ObservableProperty]
        private bool isHomeTeamFavorited;

        [ObservableProperty]
        private bool isAwayTeamFavorited;

        public bool IsHomeTeamNotFavorited => !IsHomeTeamFavorited;

        public bool IsAwayTeamNotFavorited => !IsAwayTeamFavorited;

        // Prognoza tacnog rezultata - moze se dodati/menjati samo dok je mec "Upcoming"
        // (backend to i sam proverava), a kad je mec zavrsen prikazuje se obojena kao pogodak/promasaj.
        [ObservableProperty]
        private bool isPredictionEditorOpen;

        [ObservableProperty]
        private string predictionHomeInput = string.Empty;

        [ObservableProperty]
        private string predictionAwayInput = string.Empty;

        [ObservableProperty]
        private PredictionDTO? myPrediction;

        public bool CanPredict => Details?.Status == "Upcoming";

        public bool HasPrediction => MyPrediction is not null;

        // "Your prediction: 2-1" + osvojeni bodovi kad je mec zavrsen (ishod i bodove odredjuje server).
        public string PredictionDisplay => MyPrediction switch
        {
            null => string.Empty,
            { Outcome: "Void" } p => $"Your prediction: {p.PredictedHomeScore}-{p.PredictedAwayScore} (match cancelled - no points)",
            { Points: > 0 } p => $"Your prediction: {p.PredictedHomeScore}-{p.PredictedAwayScore} (+{p.Points})",
            var p => $"Your prediction: {p.PredictedHomeScore}-{p.PredictedAwayScore}"
        };

        // Tacan rezultat zeleno, pobednik narandzasto, promasaj crveno; otkazan sivo; do ishoda neutralno.
        public Color PredictionColor => MyPrediction?.Outcome switch
        {
            "Exact" => PredictionColors.Exact,
            "Close" => PredictionColors.Close,
            "Winner" => PredictionColors.Winner,
            "Missed" => PredictionColors.Missed,
            "Void" => PredictionColors.Neutral,
            _ => Color.FromArgb("#1A1A2E")
        };

        // Pojasnjenje bodovanja ispod "Predict score" - vidi se dok se moze prognozirati ili vec postoji prognoza.
        public bool ShowPredictionHelp => CanPredict || HasPrediction;

        // Najvise cifara u polju za prognozu: 3 (0-999), a za sport sa ogranicenjem (odbojka 0-3) samo toliko cifara
        // koliko ima najveci dozvoljen broj.
        public int PredictionMaxLength => Details?.MaxPredictedScore is int max ? max.ToString().Length : 3;

        // Polje prihvata SAMO cifre (nema minusa/tacke), a kod sporta sa ogranicenjem (odbojka) samo 0..MaxPredictedScore -
        // nedozvoljen unos se odmah odbacuje. Server isto proverava (SavePrediction), ovo je samo udobnost.
        private string SanitizeScore(string? value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());

            if (digits.Length == 0)
                return digits;

            if (digits.Length > PredictionMaxLength)
                digits = digits[..PredictionMaxLength];

            if (Details?.MaxPredictedScore is int max && int.Parse(digits) > max)
                return string.Empty;

            return digits;
        }

        partial void OnPredictionHomeInputChanged(string value)
        {
            var cleaned = SanitizeScore(value);
            if (cleaned != value)
                PredictionHomeInput = cleaned;
        }

        partial void OnPredictionAwayInputChanged(string value)
        {
            var cleaned = SanitizeScore(value);
            if (cleaned != value)
                PredictionAwayInput = cleaned;
        }

        // Link "How are points scored?" -> stranica sa pravilima (sport ovog meca je tamo istaknut).
        [RelayCommand]
        private async Task OpenScoringRulesAsync()
        {
            await Shell.Current.GoToAsync($"scoringrules?sportId={Details?.SportId ?? 0}");
        }
        #endregion

        #region Methods
        partial void OnEventIdChanged(long value)
        {
            _ = LoadEventDetailsCommand.ExecuteAsync(null);
        }

        partial void OnDetailsChanged(EventDetailsDTO? value)
        {
            OnPropertyChanged(nameof(HeaderRightText));
            OnPropertyChanged(nameof(IsLive));
            OnPropertyChanged(nameof(ShowDetailedStatus));
            OnPropertyChanged(nameof(HasProgress));
            OnPropertyChanged(nameof(DetailedStatusDisplay));
            OnPropertyChanged(nameof(HasRound));
            OnPropertyChanged(nameof(HasSeason));
            OnPropertyChanged(nameof(HasLeagueName));
            OnPropertyChanged(nameof(HasVenue));
            OnPropertyChanged(nameof(HasLineups));
            OnPropertyChanged(nameof(HasStatistics));
            OnPropertyChanged(nameof(HomeLineup));
            OnPropertyChanged(nameof(AwayLineup));
            OnPropertyChanged(nameof(HasBroadcasts));
            IsBroadcastsExpanded = false;
            OnPropertyChanged(nameof(BroadcastRows));
            OnPropertyChanged(nameof(HasMoreBroadcastRows));
            OnPropertyChanged(nameof(ShowAllBroadcastsText));
            OnPropertyChanged(nameof(HasTimeline));
            OnPropertyChanged(nameof(HasFullTimeline));
            OnPropertyChanged(nameof(HasAnyDetailData));
            OnPropertyChanged(nameof(HasHomeRedCard));
            OnPropertyChanged(nameof(HasAwayRedCard));
            OnPropertyChanged(nameof(CanPredict));
            OnPropertyChanged(nameof(ShowPredictionHelp));
            OnPropertyChanged(nameof(PredictionMaxLength));
            OnPropertyChanged(nameof(PredictionColor));

            // Ako nema postave (npr. mec jos nije poceo), a ima statistike - default tab mora
            // da bude Statistics, jer bi inace prikaz bio prazan. Isto tako dalje niz listu.
            SelectedTab = HasLineups ? "Lineup" : HasStatistics ? "Statistics" : HasBroadcasts ? "Broadcasts" : "FullTimeline";
        }

        partial void OnSelectedTabChanged(string value)
        {
            OnPropertyChanged(nameof(IsLineupSelected));
            OnPropertyChanged(nameof(IsStatisticsSelected));
            OnPropertyChanged(nameof(IsBroadcastsSelected));
            OnPropertyChanged(nameof(IsFullTimelineSelected));
        }

        partial void OnIsHomeTeamFavoritedChanged(bool value)
            => OnPropertyChanged(nameof(IsHomeTeamNotFavorited));

        partial void OnIsAwayTeamFavoritedChanged(bool value)
            => OnPropertyChanged(nameof(IsAwayTeamNotFavorited));

        partial void OnMyPredictionChanged(PredictionDTO? value)
        {
            OnPropertyChanged(nameof(HasPrediction));
            OnPropertyChanged(nameof(ShowPredictionHelp));
            OnPropertyChanged(nameof(PredictionDisplay));
            OnPropertyChanged(nameof(PredictionColor));
        }

        [RelayCommand]
        private async Task TogglePredictionEditorAsync()
        {
            if (!await _authService.IsLoggedInAsync())
            {
                await Shell.Current.GoToAsync("login");
                return;
            }

            if (!IsPredictionEditorOpen && MyPrediction is not null)
            {
                PredictionHomeInput = MyPrediction.PredictedHomeScore.ToString();
                PredictionAwayInput = MyPrediction.PredictedAwayScore.ToString();
            }

            IsPredictionEditorOpen = !IsPredictionEditorOpen;
        }

        [RelayCommand]
        private async Task SavePredictionAsync()
        {
            if (!int.TryParse(PredictionHomeInput, out var home) || !int.TryParse(PredictionAwayInput, out var away)
                || home < 0 || away < 0)
            {
                ErrorMessage = "Enter valid scores (whole numbers, 0 or more).";
                return;
            }

            ErrorMessage = string.Empty;
            try
            {
                await _predictionApiService.SavePredictionAsync(EventId, home, away);
                MyPrediction = new PredictionDTO { PredictedHomeScore = home, PredictedAwayScore = away };
                IsPredictionEditorOpen = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to save prediction (the match may have already started).";
                System.Diagnostics.Debug.WriteLine($"SavePredictionAsync failed: {ex}");
            }
        }

        [RelayCommand]
        private async Task TeamSelected(long teamId)
        {
            await Shell.Current.GoToAsync($"teampage?teamId={teamId}");
        }

        [RelayCommand]
        private async Task LoadEventDetailsAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                Details = await _eventDetailsApiService.GetEventDetailsAsync(EventId);
                await LoadFavoriteStatusAsync();
                await LoadPredictionAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load event details.";
                System.Diagnostics.Debug.WriteLine($"LoadEventDetailsAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Preskace se potpuno za neulogovanog korisnika - nema smisla gadjati [Authorize]
        // endpoint (samo bi dobili 401 bez ikakve svrhe).
        private async Task LoadFavoriteStatusAsync()
        {
            if (Details is null || !await _authService.IsLoggedInAsync())
                return;

            try
            {
                var homeTask = _teamApiService.IsTeamFavoritedAsync(Details.HomeTeamId);
                var awayTask = _teamApiService.IsTeamFavoritedAsync(Details.AwayTeamId);
                await Task.WhenAll(homeTask, awayTask);

                IsHomeTeamFavorited = homeTask.Result;
                IsAwayTeamFavorited = awayTask.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFavoriteStatusAsync failed: {ex}");
            }
        }

        private async Task LoadPredictionAsync()
        {
            if (!await _authService.IsLoggedInAsync())
            {
                MyPrediction = null;
                return;
            }

            try
            {
                MyPrediction = await _predictionApiService.GetMyPredictionAsync(EventId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadPredictionAsync failed: {ex}");
            }
        }
        #endregion
    }
}
