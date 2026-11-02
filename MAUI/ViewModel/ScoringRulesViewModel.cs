using CommunityToolkit.Mvvm.ComponentModel;
using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MAUI.ViewModel
{
    // Stranica "How points are scored": opsta objasnjenja + kartica po sportu (iz tabele SportScoringRules, sa primerima
    // izracunatim na serveru). Sport meca iz kog je korisnik dosao (sportId) je istaknut i prvi na listi.
    [QueryProperty(nameof(SportId), "sportId")]
    public partial class ScoringRulesViewModel : ObservableObject
    {
        private readonly IScoringRulesApiService _scoringRulesApiService;

        public ScoringRulesViewModel(IScoringRulesApiService scoringRulesApiService)
        {
            _scoringRulesApiService = scoringRulesApiService;
        }

        [ObservableProperty]
        private long sportId;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ObservableCollection<SportScoringDTO> Sports { get; } = new();

        // Poziva stranica pri otvaranju (SportId je do tada vec postavljen preko Shell query parametra).
        public async Task LoadAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var rules = await _scoringRulesApiService.GetRulesAsync();

                foreach (var rule in rules)
                    rule.IsCurrent = rule.SportId == SportId;

                Sports.Clear();
                foreach (var rule in rules.OrderByDescending(r => r.IsCurrent).ThenBy(r => r.SportName))
                    Sports.Add(rule);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load the scoring rules.";
                System.Diagnostics.Debug.WriteLine($"ScoringRulesViewModel.LoadAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
