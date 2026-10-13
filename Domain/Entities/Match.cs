using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Match : SyncedEntity.SyncedEntity
    {
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public long LeagueId { get; set; }
        public long HomeTeamId { get; set; }
        public long AwayTeamId { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public string Season { get; set; }
        public string Round { get; set; }
        public long? VenueId { get; set; }
        public string Status { get; set; }
        public string? Progress { get; set; }

        // Postavlja se TEK kad SyncMatchDetailsAsync uspesno zavrsi CEO posao (i statistiku
        // i postavu) za ovaj mec, bez izuzetka - ne "ima bar jedan red" kao ranije, sto bi
        // trajno preskocilo mec ako je prethodni pokusaj pukao na pola posla.
        public DateTime? DetailsSyncedAt { get; set; }

        // Kad je poslednji put uspesno proveren TV raspored ovog meca (i kad provajder vrati PRAZNO -
        // "nema prenosa" se ne razlikuje od "jos nije objavljeno"). Po njemu EventTvBackgroundService
        // odlucuje da li je mec "na redu" za osvezavanje, zavisno od toga koliko je blizu pocetka.
        public DateTime? TvSyncedAt { get; set; }

        // Kad je poslednji put proveren tok meca (EventTimeline). "Provereno", ne "kompletno" - tok se
        // pojavljuje sa zakasnjenjem posle kraja meca, pa EventTimelineBackgroundService po ovome i
        // starosti meca odlucuje kad opet pokusati (i za mecove kojima je provajder vratio prazno).
        public DateTime? TimelineSyncedAt { get; set; }

        // Dedup flagovi za push podsetnike (MatchNotificationBackgroundService) - bez njih bi
        // svaki 5-minutni ciklus ponovo slao isti podsetnik dok god je mec u [now, now+24h/1h].
        public DateTime? Reminder24hSentAt { get; set; }
        public DateTime? Reminder1hSentAt { get; set; }


        public League League { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public ICollection<Prediction>? Predictions { get; set; }
        public Venue Venue { get; set; }
        public ICollection<EventStatistic> Statistics { get; set; }
        public ICollection<EventLineup> Lineups { get; set; }
        public ICollection<EventTv> TvBroadcasts { get; set; }
        public ICollection<EventTimeline> Timeline { get; set; }
    }
}
