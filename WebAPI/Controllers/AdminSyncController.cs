using Infrastructure.Sync;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/sync")]
    public class AdminSyncController : ControllerBase
    {
        private readonly ISportsDbSyncService _syncService;
        private readonly ILogger<AdminSyncController> _logger;

        public AdminSyncController(ISportsDbSyncService syncService, ILogger<AdminSyncController> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        [HttpPost("all")]
        public async Task<IActionResult> SyncAll(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncAllAsync.");

            try
            {
                await _syncService.SyncAllAsync(ct);
                return Ok(new { message = "SyncAllAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                // Ovo bi trebalo da bude redak slucaj (SyncSportsAsync/SyncLeagueAndTeams vec
                // hvataju svoje greske interno) - ali ako nesto sasvim neocekivano probije kroz
                // sve slojeve, korisnik dobija cist odgovor umesto sirovog 500 stack trace-a.
                _logger.LogError(ex, "SyncAllAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// <summary>
        /// Osvežava SAMO Sport tabelu.
        /// </summary>
        [HttpPost("sports")]
        public async Task<IActionResult> SyncSports(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncSportsAsync.");

            try
            {
                await _syncService.SyncSportsAsync(ct);
                return Ok(new { message = "SyncSportsAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncSportsAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync sportova nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// <summary>
        /// Osvežava SAMO League i Team tabele (bez Venue).
        /// </summary>
        [HttpPost("leagues-teams")]
        public async Task<IActionResult> SyncLeaguesAndTeams(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncLeagueAndTeams.");

            try
            {
                await _syncService.SyncLeagueAndTeams(ct);
                return Ok(new { message = "SyncLeagueAndTeams završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncLeagueAndTeams je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync liga/timova nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// <summary>
        /// Osvežava SAMO stadione (Venue), nezavisno od ostatka Celine 1 - korisno kad
        /// admin želi da osveži samo stadione bez ponovnog prolaska kroz sve sportove/lige/timove.
        /// </summary>
        [HttpPost("venues")]
        public async Task<IActionResult> SyncVenues(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncVenuesAsync.");

            try
            {
                await _syncService.SyncVenuesAsync(ct);
                return Ok(new { message = "SyncVenuesAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncVenuesAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync stadiona nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// <summary>
        /// Celina 2 - sastav igraca po timovima. Za rucno testiranje pre nego napravimo
        /// pozadinski servis sa dnevnim ritmom.
        /// </summary>
        [HttpPost("players")]
        public async Task<IActionResult> SyncPlayers(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncPlayersAsync.");

            try
            {
                await _syncService.SyncPlayersAsync(ct);
                return Ok(new { message = "SyncPlayersAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncPlayersAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync igraca nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Celina 2 - naredni mecevi po ligama. Za rucno testiranje pre nego napravimo
        /// pozadinski servis.
        [HttpPost("matches")]
        public async Task<IActionResult> SyncMatches(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncMatchesAsync.");

            try
            {
                await _syncService.SyncMatchesAsync(ct);
                return Ok(new { message = "SyncMatchesAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncMatchesAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync meceva nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }
        /// SKUP, JEDNOKRATAN posao - ide unazad kroz sve sezone svake lige (dok SportsDB ima
        /// podataka) i povlaci SVE mecove. Namerno odvojeno od redovnog /matches endpointa -
        /// ovo je backfill istorije, ne redovno osvezavanje. Racunaj na hiljade API poziva i
        /// dug rad (verovatno desetine minuta, mozda i sat+ zavisno od broja liga/sezona).
        [HttpPost("matches/historical")]
        public async Task<IActionResult> SyncHistoricalMatches(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncHistoricalMatchesAsync - ovo moze potrajati.");

            try
            {
                await _syncService.SyncHistoricalMatchesAsync(ct);
                return Ok(new { message = "SyncHistoricalMatchesAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncHistoricalMatchesAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Istorijski sync meceva nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Istorija dresova po timovima (retko se menja).
        [HttpPost("team-equipment")]
        public async Task<IActionResult> SyncTeamEquipment(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncTeamEquipmentAsync.");

            try
            {
                await _syncService.SyncTeamEquipmentAsync(ct);
                return Ok(new { message = "SyncTeamEquipmentAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncTeamEquipmentAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync dresova nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// SKUP posao - statistika + postava za SVE završene mečeve koji još nemaju statistiku
        /// Broj mečeva raste tokom vremena (istorijski backfill ih je napunio hiljadama) -
        /// namerno ručno okidano, ne ide u redovan ciklus
        [HttpPost("matches/details")]
        public async Task<IActionResult> SyncMatchDetails(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncMatchDetailsAsync - ovo moze potrajati.");

            try
            {
                await _syncService.SyncMatchDetailsAsync(ct);
                return Ok(new { message = "SyncMatchDetailsAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncMatchDetailsAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync detalja meceva nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// SKUP posao - karijerna statistika, transferi, trofeji i milestone-i za SVAKOG
        /// igraca u bazi (4 poziva po igracu). Namerno rucno okidano, retko se menja.
        [HttpPost("players/career-data")]
        public async Task<IActionResult> SyncPlayerCareerData(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncPlayerCareerDataAsync - ovo moze potrajati.");

            try
            {
                await _syncService.SyncPlayerCareerDataAsync(ct);
                return Ok(new { message = "SyncPlayerCareerDataAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncPlayerCareerDataAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync karijernih podataka nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Samo karijerna statistika 
        [HttpPost("players/statistics")]
        public async Task<IActionResult> SyncPlayerStatistics(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncPlayerStatisticsAsync.");

            try
            {
                await _syncService.SyncPlayerStatisticsAsync(ct);
                return Ok(new { message = "SyncPlayerStatisticsAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncPlayerStatisticsAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync statistike igraca nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Samo istorija transfera 
        [HttpPost("players/former-teams")]
        public async Task<IActionResult> SyncPlayerFormerTeams(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncPlayerFormerTeamsAsync.");

            try
            {
                await _syncService.SyncPlayerFormerTeamsAsync(ct);
                return Ok(new { message = "SyncPlayerFormerTeamsAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncPlayerFormerTeamsAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync transfera nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Samo trofeji igraca 
        [HttpPost("players/honours")]
        public async Task<IActionResult> SyncPlayerHonours(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncPlayerHonoursAsync.");

            try
            {
                await _syncService.SyncPlayerHonoursAsync(ct);
                return Ok(new { message = "SyncPlayerHonoursAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncPlayerHonoursAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync trofeja nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Samo milestone-i igraca
        [HttpPost("players/milestones")]
        public async Task<IActionResult> SyncPlayerMilestones(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncPlayerMilestonesAsync.");

            try
            {
                await _syncService.SyncPlayerMilestonesAsync(ct);
                return Ok(new { message = "SyncPlayerMilestonesAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncPlayerMilestonesAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync milestone-a nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }


        /// Live statistika meceva u "prozoru" [EventDate, EventDate+3h]. Za rucno testiranje -
        /// automatski se okida na ~2 min preko MatchStatisticsTrackerService.
        [HttpPost("matches/live-statistics")]
        public async Task<IActionResult> SyncLiveMatchStatistics(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncLiveMatchStatisticsAsync.");

            try
            {
                await _syncService.SyncLiveMatchStatisticsAsync(ct);
                return Ok(new { message = "SyncLiveMatchStatisticsAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncLiveMatchStatisticsAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Live sync statistike nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }


        /// Postava pocev od 2 dana pre meca. Za rucno testiranje - automatski se okida na
        /// ~2h preko PreMatchLineupBackgroundService.
        [HttpPost("matches/upcoming-lineups")]
        public async Task<IActionResult> SyncUpcomingLineups(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncUpcomingLineupsAsync.");

            try
            {
                await _syncService.SyncUpcomingLineupsAsync(ct);
                return Ok(new { message = "SyncUpcomingLineupsAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncUpcomingLineupsAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync unapred-postave nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }


        /// osvežavanje ide po-završenom-meču (SyncLiveMatchStatisticsAsync) - ovo je za
        /// takođe ručno "osveži sve odjednom" 
        [HttpPost("standings")]
        public async Task<IActionResult> SyncStandings(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncStandingsAsync.");

            try
            {
                await _syncService.SyncStandingsAsync(ct);
                return Ok(new { message = "SyncStandingsAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncStandingsAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync tabele lige nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// SKUP, JEDNOKRATAN posao - ide unazad kroz sve sezone svake lige (dok SportsDB ima
        /// podataka) i povlači tabelu (Standings) za svaku. Koristi V1 API (niži rate limit,
        /// 100/min) - računaj na dug rad zavisno od broja liga/sezona.
        [HttpPost("standings/historical")]
        public async Task<IActionResult> SyncHistoricalStandings(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncHistoricalStandingsAsync - ovo moze potrajati.");

            try
            {
                await _syncService.SyncHistoricalStandingsAsync(ct);
                return Ok(new { message = "SyncHistoricalStandingsAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncHistoricalStandingsAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Istorijski sync tabele lige nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Kanali (lookup/event_tv/{id}) za mecove iz Match tabele. ?mode=All (podrazumevano) obradjuje
        /// SVE mecove; ?mode=Unsynced samo one kojima TvSyncedAt jos nije postavljen (nastavak posle
        /// prekida); ?mode=Repair samo mecove sa redovima koje je napisala stara verzija (pogresan
        /// BroadcastTime). Ponovno pokretanje je bezbedno - bez duplikata. Buduce mecove inace
        /// automatski osvezava EventTvBackgroundService.
        [HttpPost("matches/tv")]
        public async Task<IActionResult> SyncEventTv([FromQuery] EventTvSyncMode mode = EventTvSyncMode.All, CancellationToken ct = default)
        {
            _logger.LogInformation("Ručno pokrenut SyncEventTvAsync (mode={Mode}).", mode);

            try
            {
                await _syncService.SyncEventTvAsync(mode, ct);
                return Ok(new { message = "SyncEventTvAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncEventTvAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync TV prenosa nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Tok meca (golovi, kartoni, izmene - lookup/event_timeline/{id}) za SVE zavrsene mecove u bazi.
        /// Podrazumevano samo sportovi za koje je tok potvrdjen (fudbal); ?allSports=true probava sve.
        /// ?mode=Unsynced obradjuje samo mecove kojima TimelineSyncedAt jos nije postavljen (nastavak posle
        /// prekida). Ponovno pokretanje je bezbedno - bez duplikata. Poslednja 3 dana inace automatski
        /// osvezava EventTimelineBackgroundService.
        [HttpPost("matches/timeline")]
        public async Task<IActionResult> SyncEventTimeline([FromQuery] TimelineSyncMode mode = TimelineSyncMode.All, [FromQuery] bool allSports = false, CancellationToken ct = default)
        {
            _logger.LogInformation("Ručno pokrenut SyncEventTimelineAsync (mode={Mode}, allSports={AllSports}) - ovo moze potrajati.", mode, allSports);

            try
            {
                await _syncService.SyncEventTimelineAsync(mode, allSports, ct);
                return Ok(new { message = "SyncEventTimelineAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncEventTimelineAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Sync toka meca nije uspeo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }

        /// Obogacuje "stub" igrace (TeamId == null, napravljeni iz postave meca) sa punim
        /// profilom preko lookup/player/{id} - bezbedno, po tacnom ExternalId.
        [HttpPost("players/enrich-stubs")]
        public async Task<IActionResult> SyncStubPlayerProfiles(CancellationToken ct)
        {
            _logger.LogInformation("Ručno pokrenut SyncStubPlayerProfilesAsync.");

            try
            {
                await _syncService.SyncStubPlayerProfilesAsync(ct);
                return Ok(new { message = "SyncStubPlayerProfilesAsync završen - proveri Logs tabelu za eventualne greške po pojedinačnim stavkama." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncStubPlayerProfilesAsync je bacio neuhvacenu gresku.");
                return StatusCode(500, new { message = "Obogacivanje stub igraca nije uspelo. Proveri konzolu/Logs tabelu za detalje." });
            }
        }
    }
}
