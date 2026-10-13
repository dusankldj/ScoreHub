using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using MAUI.Services.Interfaces;

namespace MAUI.ViewModel
{
    // Grupa statistike po sezoni - vidi PlayerViewModel.StatisticsBySeason.
    public class PlayerStatisticsSeasonGroup
    {
        public string Season { get; set; } = string.Empty;
        public List<PlayerStatistics> Items { get; set; } = new();
    }

    [QueryProperty(nameof(PlayerId), "playerId")]
    public partial class PlayerViewModel : ObservableObject
    {
        #region Fields
        private readonly IPlayerApiService _playerApiService;
        #endregion

        #region Constructor
        public PlayerViewModel(IPlayerApiService playerApiService)
        {
            _playerApiService = playerApiService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private long playerId;

        [ObservableProperty]
        private PlayerDTO? details;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public bool HasTeam => !string.IsNullOrWhiteSpace(Details?.Team);

        public bool HasBirthLocation => !string.IsNullOrWhiteSpace(Details?.BirthLocation);

        public bool HasStatus => !string.IsNullOrWhiteSpace(Details?.Status);

        public bool HasWorth => !string.IsNullOrWhiteSpace(Details?.Worth);

        public bool HasWeight => !string.IsNullOrWhiteSpace(Details?.Weight);

        public bool HasHeight => !string.IsNullOrWhiteSpace(Details?.Height);

        public bool HasDescription => !string.IsNullOrWhiteSpace(Details?.Description);

        public bool HasStatistics => Details?.Statistics?.Count > 0;

        public bool HasHonours => Details?.Honours?.Count > 0;

        public bool HasMilestones => Details?.Milestones?.Count > 0;

        public bool HasFormerTeams => Details?.FormerTeams?.Count > 0;

        public bool HasSocials => Details?.HasAnySocial == true;

        public bool HasAnyTabData => HasStatistics || HasHonours || HasMilestones || HasFormerTeams || HasSocials;

        // Podrazumevano Statistics - ako igrac nema statistiku, pada na prvi tab koji ima podataka.
        [ObservableProperty]
        private string selectedTab = "Statistics";

        public bool IsStatisticsSelected => SelectedTab == "Statistics";

        public bool IsHonoursSelected => SelectedTab == "Honours";

        public bool IsMilestonesSelected => SelectedTab == "Milestones";

        public bool IsFormerTeamsSelected => SelectedTab == "FormerTeams";

        public bool IsSocialsSelected => SelectedTab == "Socials";

        public List<PlayerStatisticsSeasonGroup> StatisticsBySeason { get; private set; } = new();
        #endregion

        #region Methods
        partial void OnPlayerIdChanged(long value)
        {
            _ = LoadPlayerDetailsCommand.ExecuteAsync(null);
        }

        partial void OnDetailsChanged(PlayerDTO? value)
        {
            OnPropertyChanged(nameof(HasTeam));
            OnPropertyChanged(nameof(HasBirthLocation));
            OnPropertyChanged(nameof(HasStatus));
            OnPropertyChanged(nameof(HasWorth));
            OnPropertyChanged(nameof(HasWeight));
            OnPropertyChanged(nameof(HasHeight));
            OnPropertyChanged(nameof(HasDescription));
            OnPropertyChanged(nameof(HasStatistics));
            OnPropertyChanged(nameof(HasHonours));
            OnPropertyChanged(nameof(HasMilestones));
            OnPropertyChanged(nameof(HasFormerTeams));
            OnPropertyChanged(nameof(HasSocials));
            OnPropertyChanged(nameof(HasAnyTabData));

            StatisticsBySeason = Details?.Statistics
                ?.GroupBy(s => s.Season ?? string.Empty)
                .OrderBy(g => ExtractLeadingYear(g.Key))
                .Select(g => new PlayerStatisticsSeasonGroup { Season = g.Key, Items = g.ToList() })
                .ToList() ?? new List<PlayerStatisticsSeasonGroup>();
            OnPropertyChanged(nameof(StatisticsBySeason));

            SelectedTab = HasStatistics ? "Statistics"
                : HasHonours ? "Honours"
                : HasMilestones ? "Milestones"
                : HasFormerTeams ? "FormerTeams"
                : "Socials";
        }

        partial void OnSelectedTabChanged(string value)
        {
            OnPropertyChanged(nameof(IsStatisticsSelected));
            OnPropertyChanged(nameof(IsHonoursSelected));
            OnPropertyChanged(nameof(IsMilestonesSelected));
            OnPropertyChanged(nameof(IsFormerTeamsSelected));
            OnPropertyChanged(nameof(IsSocialsSelected));
        }

        // "2025-2026" -> 2025, "2020" -> 2020, nepoznat/prazan format -> 0 (ide na dno kad se
        // sortira opadajuce).
        private static int ExtractLeadingYear(string season)
        {
            if (string.IsNullOrWhiteSpace(season))
                return 0;

            var digits = new string(season.TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(digits, out var year) ? year : 0;
        }

        [RelayCommand]
        private async Task OpenSocialLink(string? link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return;

            var url = link.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? link : $"https://{link}";

            try
            {
                await Launcher.Default.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenSocialLink failed for '{url}': {ex}");
            }
        }

        [RelayCommand]
        private async Task LoadPlayerDetailsAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                Details = await _playerApiService.GetPlayerDetailsAsync(PlayerId);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load player details.";
                System.Diagnostics.Debug.WriteLine($"LoadPlayerDetailsAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
