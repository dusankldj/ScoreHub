using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MAUI.ViewModel
{
    public class StandingLegendItem
    {
        public string ColorHex { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    [QueryProperty(nameof(LeagueId), "leagueId")]
    public partial class LeagueViewModel : ObservableObject
    {
        #region Fields
        private readonly ILeagueApiService _leagueApiService;

        // Redosled je namerno fiksan (ne nasumican) - ista boja se dobija na svaki refresh
        // dok je paleta ista, olaksava testiranje/screenshot poredjenje.
        private static readonly string[] LegendPalette =
        {
            "#2E7D32", "#1565C0", "#EF6C00", "#C62828",
            "#6A1B9A", "#00838F", "#F9A825", "#4E342E"
        };
        #endregion

        #region Constructor
        public LeagueViewModel(ILeagueApiService leagueApiService)
        {
            _leagueApiService = leagueApiService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private long leagueId;

        [ObservableProperty]
        private LeagueDetailsDTO? details;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public bool HasStandings => Details?.Standings?.Count > 0;

        // Rezerva - samo ako standings JOS NISU sinhronizovani za ovu ligu.
        public bool HasTeams => !HasStandings && Details?.Teams?.Count > 0;

        public ObservableCollection<StandingLegendItem> Legend { get; } = new();
        #endregion

        #region Methods
        partial void OnLeagueIdChanged(long value)
        {
            _ = LoadLeagueDetailsCommand.ExecuteAsync(null);
        }

        partial void OnDetailsChanged(LeagueDetailsDTO? value)
        {
            OnPropertyChanged(nameof(HasStandings));
            OnPropertyChanged(nameof(HasTeams));
            BuildLegendAndAssignColors();
        }

        // Ista Description vrednost -> ista boja kroz celu tabelu (i u legendi). Ne dolazi sa
        // API-ja gotovo - racuna se ovde JEDNOM po ucitavanju, ne u XAML-u/converteru.
        private void BuildLegendAndAssignColors()
        {
            Legend.Clear();

            var standings = Details?.Standings;
            if (standings is not { Count: > 0 })
                return;

            var distinctDescriptions = standings
                .Select(s => s.Description)
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Distinct()
                .ToList();

            var colorByDescription = new Dictionary<string, string>();
            for (var i = 0; i < distinctDescriptions.Count; i++)
            {
                colorByDescription[distinctDescriptions[i]!] = LegendPalette[i % LegendPalette.Length];
            }

            foreach (var standing in standings)
            {
                standing.DescriptionColorHex = standing.HasDescription && colorByDescription.TryGetValue(standing.Description!, out var color)
                    ? color
                    : null;
            }

            foreach (var kvp in colorByDescription)
            {
                Legend.Add(new StandingLegendItem { ColorHex = kvp.Value, Description = kvp.Key });
            }
        }

        [RelayCommand]
        private async Task LoadLeagueDetailsAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                Details = await _leagueApiService.GetLeagueDetailsAsync(LeagueId);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load league details.";
                System.Diagnostics.Debug.WriteLine($"LoadLeagueDetailsAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
