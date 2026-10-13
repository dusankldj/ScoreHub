using Infrastructure.Integrations.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Integrations
{
    public interface ISportsDbClient
    {
        Task<SportsDbLeague?> GetLeagueAsync(string externalLeagueId, CancellationToken ct = default);

        Task<List<SportsDbTeam>> GetTeamsForLeagueAsync(string externalLeagueId, CancellationToken ct = default);

        Task<List<SportsDbSport>> GetSportsAsync(CancellationToken ct = default);

        /// <summary>
        /// Pojedinačni detalji tima (lookup/team/{id}) - jedini izvor imena stadiona
        /// (StrStadium), koje bulk lista timova (GetTeamsForLeagueAsync) ne vraća.
        /// </summary>
        Task<SportsDbTeam?> GetTeamDetailAsync(string externalTeamId, CancellationToken ct = default);

        /// <summary>
        /// Pretraga stadiona po IMENU (search/venue/{ime}) - nema pretragu po ID-ju,
        /// samo po tekstu naziva.
        /// </summary>
        Task<SportsDbVenue?> SearchVenueAsync(string venueName, CancellationToken ct = default);

        /// <summary>
        /// Sastav tima (list/players/{idTeam}) - otkrivanje igraca po timu.
        /// </summary>
        Task<List<SportsDbPlayer>> GetPlayersForTeamAsync(string externalTeamId, CancellationToken ct = default);

        /// <summary>
        /// Pojedinačni detalji igrača (lookup/player/{id}) - jedini izvor StrLastName,
        /// StrStatus, StrWeight, StrHeight, StrDescriptionEN, StrBirthLocation, društvenih
        /// mreža - GetPlayersForTeamAsync (list/players) vraća samo mršav podskup.
        /// </summary>
        Task<SportsDbPlayer?> GetPlayerDetailAsync(string externalPlayerId, CancellationToken ct = default);

        /// <summary>
        /// Naredni meCevi lige (schedule/next/league/{id}) - otkrivanje NOVIH meceva.
        /// Potvrdjeno u praksi - vraca ograničen broj (desetak) narednih mečeva po ligi.
        /// </summary>
        Task<List<SportsDbMatch>> GetNextMatchesForLeagueAsync(string externalLeagueId, CancellationToken ct = default);

        /// <summary>
        /// Odigrani mečevi lige (schedule/previous/league/{id}) - potvrdjeno, vraća
        /// ograničen broj (10) poslednjih odigranih mečeva po ligi.
        /// </summary>
        Task<List<SportsDbMatch>> GetPreviousMatchesForLeagueAsync(string externalLeagueId, CancellationToken ct = default);

        /// <summary>
        /// CELA sezona meceva lige (schedule/league/{id}/{sezona}) - potvrdjeno, vraca
        /// SVE mecove (odigrane i zakazane) date sezone u JEDNOM pozivu. Ovo je glavni
        /// izvor za otkrivanje meceva - next/previous ostaju dostupni za lakse/uze provere.
        /// </summary>
        Task<List<SportsDbMatch>> GetSeasonMatchesForLeagueAsync(string externalLeagueId, string season, CancellationToken ct = default);

        /// <summary>
        /// Pojedinacni detalji meca (lookup/event/{id}) - potvrdjeno, koristi se za
        /// osvezavanje/detaljan fetch kad vec znamo idEvent.
        /// </summary>
        Task<SportsDbMatch?> GetEventDetailAsync(string externalEventId, CancellationToken ct = default);

        /// <summary>
        /// Statistika meca (lookup/event_stats/{id}) - dostupna uglavnom za zavrsene mecove.
        /// </summary>
        Task<List<SportsDbEventStatistic>> GetEventStatisticsAsync(string externalEventId, CancellationToken ct = default);

        /// <summary>
        /// Postava meca (lookup/event_lineup/{id}) - ko je igrao, pozicija, starter/rezerva.
        /// </summary>
        Task<List<SportsDbEventLineup>> GetEventLineupAsync(string externalEventId, CancellationToken ct = default);

        /// <summary>
        /// Dresovi tima kroz sezone (lookup/team_equipment/{id}).
        /// </summary>
        Task<List<SportsDbTeamEquipment>> GetTeamEquipmentAsync(string externalTeamId, CancellationToken ct = default);

        /// <summary>
        /// Karijerna statistika igrača po sezoni/ligi (lookup/player_stats/{id}).
        /// </summary>
        Task<List<SportsDbPlayerStatistic>> GetPlayerStatisticsAsync(string externalPlayerId, CancellationToken ct = default);

        /// <summary>
        /// Istorija transfera igrača (lookup/player_teams/{id}).
        /// </summary>
        Task<List<SportsDbPlayerFormerTeam>> GetPlayerFormerTeamsAsync(string externalPlayerId, CancellationToken ct = default);

        /// <summary>
        /// Osvojeni trofeji igrača (lookup/player_honours/{id}).
        /// </summary>
        Task<List<SportsDbPlayerHonour>> GetPlayerHonoursAsync(string externalPlayerId, CancellationToken ct = default);

        /// <summary>
        /// Milestone-i u karijeri igrača (lookup/player_milestones/{id}).
        /// </summary>
        Task<List<SportsDbPlayerMilestone>> GetPlayerMilestonesAsync(string externalPlayerId, CancellationToken ct = default);

        /// <summary>
        /// Tabela lige (lookuptable.php?l={id}&amp;s={sezona}) - SAMO V1 API ima ovu rutu,
        /// V2 nema ekvivalent. Koristi isti ApiKey kao V2 klijent, samo kao deo putanje
        /// (V1 stil autentifikacije) umesto X-API-KEY header-a.
        /// </summary>
        Task<List<SportsDbStanding>> GetLeagueStandingsAsync(string externalLeagueId, string? season = null, CancellationToken ct = default);

        /// <summary>
        /// Uzivo rezultati za CEO sport odjednom (livescore/{sport}) - jedini izvor koji stvarno
        /// popunjava strProgress (lookup/event ga uvek vraca praznog). "sport" je URL-segment
        /// oblik ("american_football", ne "American Football") - vidi SportsDbSyncService
        /// za konverziju iz naseg prijateljskog imena.
        /// </summary>
        Task<List<SportsDbLiveScore>> GetLiveScoresAsync(string sport, CancellationToken ct = default);

        /// <summary>
        /// TV kanali koji prenose dati mec (lookup/event_tv/{id}).
        /// </summary>
        Task<List<SportsDbEventTv>> GetEventTvAsync(string externalEventId, CancellationToken ct = default);

        /// <summary>
        /// Tok meca - golovi, kartoni, izmene (lookup/event_timeline/{id}). Dostupno tek posle kraja meca.
        /// </summary>
        Task<List<SportsDbEventTimeline>> GetEventTimelineAsync(string externalEventId, CancellationToken ct = default);
    }
}
