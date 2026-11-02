using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using System.Collections.ObjectModel;

namespace MAUI.ViewModel
{
    // Lista prognoza sa filterima (status + ishod) i "Show more / Show less" - deli je Profile (moje prognoze) i
    // UserPage (prognoze prijatelja) preko PredictionsSection. SVE prognoze stizu jednim pozivom i cuvaju se u
    // memoriji (_all), a ekranu se daje prvih Limit - filter i "Show more/less" su zato trenutni (bez novog poziva).
    public partial class PredictionsListModel : ObservableObject
    {
        private const int Limit = 5;

        private List<MyPredictionDTO> _all = new();
        private int _filteredCount;

        [ObservableProperty]
        private string title = "My predictions";

        // ---- Filter "Common" (samo UserPage): mecevi koje su prognozirali I ja I prijatelj ----
        // ShowCommonFilter ukljucuje treci padajuci meni; OtherName je ime prijatelja za prikaz poredjenja.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCommonSelected))]
        private bool showCommonFilter;

        [ObservableProperty]
        private string otherName = string.Empty;

        public IReadOnlyList<string> CommonFilterOptions { get; } = new[] { "All", "Common" };

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCommonSelected))]
        private string selectedCommonFilter = "All";

        public bool IsCommonSelected => ShowCommonFilter && SelectedCommonFilter == "Common";

        // "Head-to-head: you 12 pts - 9 pts Marko - 8 common matches" - preko SVIH zajednickih meceva, bez obzira na
        // ostale filtere. Bodovi se broje samo za zavrsene meceve (nezavrseni imaju 0).
        public string HeadToHeadText
        {
            get
            {
                var common = _all.Where(p => p.Mine is not null).ToList();
                if (common.Count == 0)
                    return "No common matches yet.";

                var mine = common.Sum(p => p.Mine!.Points);
                var theirs = common.Sum(p => p.Points);
                return $"Head-to-head: you {mine} pts \u2013 {theirs} pts {OtherName}  \u00b7  {common.Count} common {(common.Count == 1 ? "match" : "matches")}";
            }
        }

        public string CommonLegendText => $"Green: you scored more  \u00b7  Orange: {OtherName} scored more  \u00b7  Grey: tie / not finished";

        // Tri odvojene liste (isti izvor, podeljen po stanju meca) da svaka sekcija ima svoj naslov i sakrije se kad je prazna.
        public ObservableCollection<MyPredictionDTO> FinishedPredictions { get; } = new();

        public ObservableCollection<MyPredictionDTO> LivePredictions { get; } = new();

        public ObservableCollection<MyPredictionDTO> UpcomingPredictions { get; } = new();

        public bool HasFinishedPredictions => FinishedPredictions.Count > 0;

        public bool HasLivePredictions => LivePredictions.Count > 0;

        public bool HasUpcomingPredictions => UpcomingPredictions.Count > 0;

        // Naslov i filteri se vide cim korisnik IKAD ima prognozu (ne zavisi od izabranog filtera) - inace
        // bi filter sa praznim rezultatom nestao zajedno sa sekcijom i ne bi se moglo vratiti na "All".
        public bool HasAnyPredictions => _all.Count > 0;

        public IReadOnlyList<string> PredictionFilterOptions { get; } = new[] { "All", "Finished", "Live", "Upcoming" };

        [ObservableProperty]
        private string selectedPredictionFilter = "All";

        // Drugi filter - ishod prognoze. "Hit" = osvojeni bodovi (tacan rezultat ili pobednik), "Missed" = 0 bodova;
        // oba postoje samo za ZAVRSENE mecove (Hit je null dok mec traje / nije poceo / je otkazan).
        public IReadOnlyList<string> ResultFilterOptions { get; } = new[] { "All", "Hit", "Missed" };

        [ObservableProperty]
        private string selectedResultFilter = "All";

        [ObservableProperty]
        private bool isPredictionsExpanded;

        public bool HasNoPredictionsForFilter => _all.Count > 0 && _filteredCount == 0;

        // Dugme postoji cim ima vise od Limit prognoza: "Show more (N)" (N = ukupno filtrirano) / "Show less".
        public bool HasMorePredictions => _filteredCount > Limit;

        public string TogglePredictionsText => IsPredictionsExpanded ? "Show less" : $"Show more ({_filteredCount})";

        // own != null -> prijateljeve prognoze se uparuju sa MOJIMA po mecu (Mine), za filter "Common".
        public void SetItems(IEnumerable<MyPredictionDTO> items, IEnumerable<MyPredictionDTO>? own = null)
        {
            _all = items.ToList();

            if (own is not null)
            {
                var mine = own.GroupBy(p => p.EventId).ToDictionary(g => g.Key, g => g.First());
                foreach (var p in _all)
                    p.Mine = mine.GetValueOrDefault(p.EventId);
            }

            Apply();
        }

        private static void Fill<T>(ObservableCollection<T> target, IEnumerable<T> items)
        {
            target.Clear();
            foreach (var item in items)
                target.Add(item);
        }

        // Filter -> sortiranje po sekcijama -> UKUPNO ogranicenje na Limit (redom Finished, Live, Upcoming) ->
        // popunjavanje tri liste. "Show more (N)" i "No predictions" se racunaju nad FILTRIRANIM skupom.
        // Napomena: u "All" prikazu, ko ima 5+ zavrsenih vidi samo njih dok ne klikne "Show more" - za
        // Live/Upcoming sluzi filter.
        private void Apply()
        {
            IEnumerable<MyPredictionDTO> filtered = SelectedPredictionFilter switch
            {
                "Finished" => _all.Where(p => p.IsFinished),
                "Live" => _all.Where(p => p.IsLive),
                "Upcoming" => _all.Where(p => p.IsUpcoming),
                _ => _all
            };

            // "Common": samo mecevi koje je prognozirao i ulogovani korisnik; redovi tada prikazuju poredjenje.
            var common = IsCommonSelected;
            foreach (var p in _all)
            {
                p.ShowComparison = common;
                p.OtherUsername = OtherName;
            }

            if (common)
                filtered = filtered.Where(p => p.Mine is not null);

            filtered = SelectedResultFilter switch
            {
                "Hit" => filtered.Where(p => p.Hit == true),
                "Missed" => filtered.Where(p => p.Hit == false),
                _ => filtered
            };

            var finished = filtered.Where(p => p.IsFinished).OrderByDescending(p => p.EventDate).ToList();
            var live = filtered.Where(p => p.IsLive).OrderBy(p => p.EventDate).ToList();
            var upcoming = filtered.Where(p => p.IsUpcoming).OrderBy(p => p.EventDate).ToList();

            _filteredCount = finished.Count + live.Count + upcoming.Count;

            var remaining = IsPredictionsExpanded ? int.MaxValue : Limit;

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
            OnPropertyChanged(nameof(TogglePredictionsText));
            OnPropertyChanged(nameof(HeadToHeadText));
            OnPropertyChanged(nameof(CommonLegendText));
        }

        [RelayCommand]
        private void TogglePredictions() => IsPredictionsExpanded = !IsPredictionsExpanded;

        partial void OnIsPredictionsExpandedChanged(bool value) => Apply();

        // Novi filter = nov skup, pa lista krece skracena ispocetka (prvih 5 filtriranih).
        partial void OnSelectedPredictionFilterChanged(string value) => RefreshAfterFilterChange();

        partial void OnSelectedResultFilterChanged(string value) => RefreshAfterFilterChange();

        partial void OnSelectedCommonFilterChanged(string value) => RefreshAfterFilterChange();

        partial void OnOtherNameChanged(string value) => Apply();

        private void RefreshAfterFilterChange()
        {
            if (IsPredictionsExpanded)
                IsPredictionsExpanded = false; // okida Apply preko OnIsPredictionsExpandedChanged
            else
                Apply();
        }
    }
}
