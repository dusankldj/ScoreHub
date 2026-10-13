using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MAUI.DTO.Input
{
    public class MatchesDTO
    {
        public long SportId { get; set; }
        public string SportName { get; set; }

        // ObservableCollection (ne List) - HomeViewModel.MergeMatches ume da UKLONI mec koji
        // vise ne odgovara aktivnom Status filteru (npr. presao iz Live u Finished dok je
        // filter na "Live") tokom live-refresh-a; List ne bi obavestio BindableLayout o tome.
        public ObservableCollection<LeaguesDTO> Leagues { get; set; } = new();
    }

    public class LeaguesDTO
    {
        public long LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string? LeagueLogoUrl { get; set; }

        public bool HasLeagueLogo => !string.IsNullOrWhiteSpace(LeagueLogoUrl);

        public bool NoLeagueLogo => !HasLeagueLogo;

        public ObservableCollection<EventsDTO> Events { get; set; } = new();
    }

    // ObservableObject (ne obican POCO) - live-refresh AZURIRA POSTOJECE instance ovih
    // objekata umesto da pravi nove (HomeViewModel.MergeMatches), pa Label-i u XAML-u
    // moraju da budu obavesteni (PropertyChanged) da bi prikazali novu vrednost i da bi
    // ScoreFlashBehavior mogao da "vidi" promenu i pokrene animaciju.
    public partial class EventsDTO : ObservableObject
    {
        public long EventId { get; set; }
        public DateTime EventDate { get; set; }

        // Backend salje EventDate kao "goli" ISO string bez Z/offseta (npr. "2026-08-29T00:30:00")
        // - System.Text.Json ga deserijalizuje kao DateTimeKind.Unspecified, NE Utc. Direktno
        // formatiranje EventDate bi zato ispisalo UTC sat kao da je lokalni. DateTime.SpecifyKind
        // eksplicitno "pecatira" vrednost kao Utc (bez pomeranja broja) pre ToLocalTime() konverzije.
        public DateTime LocalEventDate => DateTime.SpecifyKind(EventDate, DateTimeKind.Utc).ToLocalTime();

        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeamLogoUrl { get; set; }
        public string AwayTeamLogoUrl { get; set; }

        [ObservableProperty]
        private int? homeTeamScore;

        [ObservableProperty]
        private int? awayTeamScore;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TimeDisplay))]
        private string? status = string.Empty;

        // Sirov provider kod (1H/2H/HT/S1-S4/ADB...) - vidi TimeDisplay ispod.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TimeDisplay))]
        private string? detailedStatus;

        // Minut/set brojac (npr "66", "2") - ide UZ DetailedStatus ("2H - 66'").
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TimeDisplay))]
        private string? progress;

        // Levo od home logo-a - dok je mec Live, MENJA vreme pocetka (nema vise smisla, mec
        // je vec poceo) sa preciznijim statusom "2H - 66'". Van toga (Upcoming/Finished) ostaje
        // obicno vreme. Kolona sa "Live" tekstom na kraju reda ostaje NEZAVISNA od ovoga.
        public string TimeDisplay
        {
            get
            {
                if (Status != "Live")
                    return LocalEventDate.ToString("HH:mm");

                var hasProgress = !string.IsNullOrWhiteSpace(Progress);

                // Provider jos javlja "NS" (ili nista) a minut vec tece - "NS" je zastareo, prikazi samo minut.
                if (hasProgress && (string.IsNullOrWhiteSpace(DetailedStatus) || DetailedStatus == "NS"))
                    return $"{Progress}'";

                if (string.IsNullOrWhiteSpace(DetailedStatus))
                    return LocalEventDate.ToString("HH:mm");

                return hasProgress ? $"{DetailedStatus} - {Progress}'" : DetailedStatus!;
            }
        }

        // Cisto klijentska, prolazna zastavica - NE dolazi sa servera. Postavlja se na true
        // na ~1s dok traje animacija promene rezultata, pa se sama vrati na false.
        [ObservableProperty]
        private bool isScoreFlashing;
    }
}
