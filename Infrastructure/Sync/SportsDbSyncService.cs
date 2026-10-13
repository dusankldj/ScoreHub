using DataAccess;
using Domain.Entities;
using Infrastructure.Integrations;
using Infrastructure.Integrations.DTO;
using Infrastructure.Integrations.Exceptions;
using Infrastructure.Integrations.SportsDB;
using Infrastructure.Push;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Sync
{
    public class SportsDbSyncService : ISportsDbSyncService
    {
        #region Fields
        private readonly ScoreHubDbContext _context;
        private readonly ISportsDbClient _client;
        private readonly SportsOptions _options;
        private readonly ILogger<SportsDbSyncService> _logger;
        private readonly IMatchNotificationService _matchNotificationService;
        #endregion

        #region Constructor
        public SportsDbSyncService(ScoreHubDbContext dbContext, ISportsDbClient client, IOptions<SportsOptions> options, ILogger<SportsDbSyncService> logger, IMatchNotificationService matchNotificationService)
        {
            _context = dbContext;
            _client = client;
            _options = options.Value;
            _logger = logger;
            _matchNotificationService = matchNotificationService;
        }
        #endregion

        #region Methods
            #region Sync Sports
            public async Task SyncSportsAsync(CancellationToken ct = default)
            {
                List<Infrastructure.Integrations.DTO.SportsDbSport> sportDtos;

                try
                {
                    sportDtos = await FetchListSafeAsync(() => _client.GetSportsAsync(ct), "all sports", ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetSportsAsync nije uspeo - ceo SyncSportsAsync se preskace za ovaj ciklus.");
                    await LogErrorAsync("Failed to fetch all sports from SportsDB.", ex, ct);
                    return; // nema smisla nastaviti - nemamo nista da obradjujemo
                }

                foreach (var dto in sportDtos)
                {
                    if (!_options.SportsIds.Contains(dto.IdSport))
                        continue;

                    try
                    {
                        var sport = await _context.Sports.FirstOrDefaultAsync(x => x.ExternalId == dto.IdSport, ct);

                        if (sport is null)
                        {
                            sport = new Sport { ExternalId = dto.IdSport };
                            _context.Sports.Add(sport);
                        }

                        sport.Name = dto.StrSport;
                        sport.IconUrl = dto.StrSportThumb ?? string.Empty;
                        sport.Description = dto.StrSportDescription;
                        sport.LastSyncedAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sync sporta ExternalId={ExternalSportId} nije uspeo.", dto.IdSport);
                        _context.ChangeTracker.Clear(); // reset - spreci da pokvareno stanje "zarazi" sledece iteracije
                        await LogErrorAsync($"Failed to sync Sport (ExternalId={dto.IdSport}, Name={dto.StrSport}).", ex, ct);
                    }
                }

                await _context.SaveChangesAsync(ct);
            }
        #endregion

            #region Sync Leagues and Teams
            public async Task SyncLeagueAndTeams(CancellationToken ct = default)
            {
                foreach (var externalLeagueId in _options.LeagueExternalIds)
                {
                    try
                    {
                        var dto = await FetchSingleSafeAsync(() => _client.GetLeagueAsync(externalLeagueId, ct), $"league ExternalId={externalLeagueId}", ct);

                        if (dto is null)
                        {
                            _logger.LogWarning("Liga ExternalId={ExternalLeagueId} nije pronadjena na SportsDB.", externalLeagueId);
                            continue;
                        }

                        var sport = await _context.Sports.FirstOrDefaultAsync(x => x.Name == dto.StrSport, ct);

                        if (sport is null)
                        {
                            _logger.LogWarning("Sport '{Sport}' nije pronadjen u bazi za ligu {League} - da li je SyncSportsAsync pokrenut prvi i da li je ovaj sport u SportsIds listi?", dto.StrSport, dto.StrLeague);
                            continue;
                        }

                        var league = await _context.Leagues.FirstOrDefaultAsync(x => x.ExternalId == dto.IdLeague, ct);

                        if (league is null)
                        {
                            league = new League { ExternalId = dto.IdLeague };
                            _context.Leagues.Add(league);
                        }

                        league.Name = dto.StrLeague;
                        league.Country = dto.StrCountry ?? string.Empty;
                        league.LogoUrl = dto.StrBadge ?? string.Empty;
                        league.CurrentSeason = dto.StrCurrentSeason ?? string.Empty;
                        league.SportId = sport.Id;
                        league.LastSyncedAt = DateTime.UtcNow;

                        await _context.SaveChangesAsync(ct); // treba nam league.Id pre timova

                        List<Infrastructure.Integrations.DTO.SportsDbTeam> teamDtos = await FetchListSafeAsync(
                            () => _client.GetTeamsForLeagueAsync(externalLeagueId, ct), $"teams for league ExternalId={externalLeagueId} ({dto.StrLeague})", ct);

                        foreach (var teamDto in teamDtos)
                        {
                            var team = await _context.Teams.FirstOrDefaultAsync(x => x.ExternalId == teamDto.IdTeam, ct);

                            if (team is null)
                            {
                                team = new Team { ExternalId = teamDto.IdTeam };
                                _context.Teams.Add(team);
                            }

                            team.Name = teamDto.strTeam;
                            team.ShortName = teamDto.strTeamShort;
                            team.LogoUrl = teamDto.StrBadge ?? string.Empty;
                            team.Country = teamDto.StrCountry ?? string.Empty;
                            team.LeagueId = league.Id;
                            team.LastSyncedAt = DateTime.UtcNow;
                            // team.StadiumId ostaje null - V2 team odgovor nema podatke o stadionu,
                            // popunicemo kad napravimo Venue sync
                        }

                        await _context.SaveChangesAsync(ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sync nije uspeo za ligu ExternalId={ExternalLeagueId}.", externalLeagueId);
                        _context.ChangeTracker.Clear();
                        await LogErrorAsync($"Failed to sync League and Teams (ExternalLeagueId={externalLeagueId}).", ex, ct);
                    }

                }
            }
        #endregion

            #region Sync Venues
            public async Task SyncVenuesAsync(CancellationToken ct = default)
            {
                var teams = await _context.Teams.ToListAsync(ct);

                foreach (var team in teams)
                {
                    try
                    {
                        // Ako je prethodna iteracija pozvala ChangeTracker.Clear() (posle greske),
                        // ovaj "team" objekat (ucitan JEDNOM pre petlje) je otkacen od konteksta -
                        // ponovo ga zakacujemo, inace ce team.StadiumId = ... nize TIHO propasti
                        // (bez greske, ali i bez upisa u bazu).
                        if (_context.Entry(team).State == EntityState.Detached)
                        {
                            _context.Entry(team).State = EntityState.Unchanged;
                        }

                        var teamDetail = await FetchSingleSafeAsync(() => _client.GetTeamDetailAsync(team.ExternalId, ct), $"team detail ExternalId={team.ExternalId} ({team.Name})", ct);

                        if (teamDetail is null || string.IsNullOrWhiteSpace(teamDetail.StrStadium))
                        {
                            _logger.LogInformation("No stadium name available for team ExternalId={ExternalTeamId} ({Team}) - skipping.", team.ExternalId, team.Name);
                            continue;
                        }

                        var venueDto = await FetchSingleSafeAsync(() => _client.SearchVenueAsync(teamDetail.StrStadium, ct), $"venue search '{teamDetail.StrStadium}' (team {team.Name})", ct);

                        if (venueDto is null)
                        {
                            _logger.LogInformation("Venue search returned no results for '{VenueName}' (team {Team}) - skipping.", teamDetail.StrStadium, team.Name);
                            continue;
                        }

                        var externalVenueId = venueDto.IdVenue.ToString();

                        var venue = await _context.Venues.FirstOrDefaultAsync(x => x.ExternalId == externalVenueId, ct);
                        var sport = await _context.Sports.FirstOrDefaultAsync(x => x.Name == venueDto.StrSport, ct);

                        if (venue is null)
                        {
                            if (sport is null)
                            {
                                // Novi venue, ali ne znamo mu sport - SportId je obavezan FK, ne mozemo
                                // da upisemo red bez validne vrednosti.
                                _logger.LogWarning("Sport '{Sport}' not found in database for new venue '{Venue}' - skipping.", venueDto.StrSport, venueDto.StrVenue);
                                continue;
                            }

                            venue = new Venue { ExternalId = externalVenueId, SportId = sport.Id };
                            _context.Venues.Add(venue);
                        }
                        else if (sport is not null)
                        {
                            venue.SportId = sport.Id; // osvezi ako je sad pronadjen, inace zadrzi postojecu vrednost
                        }

                        venue.Name = venueDto.StrVenue;
                        venue.Location = venueDto.StrLocation ?? string.Empty;
                        venue.Country = venueDto.StrCountry ?? string.Empty;
                        venue.ImageUrl = venueDto.StrThumb ?? string.Empty;
                        venue.LastSyncedAt = DateTime.UtcNow;

                        await _context.SaveChangesAsync(ct); // treba nam venue.Id pre povezivanja sa timom

                        team.StadiumId = venue.Id;
                        await _context.SaveChangesAsync(ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sync stadiona nije uspeo za tim ExternalId={ExternalTeamId}.", team.ExternalId);
                        _context.ChangeTracker.Clear();
                        await LogErrorAsync($"Failed to sync Venue for Team (ExternalId={team.ExternalId}, Name={team.Name}).", ex, ct);
                    }
                }
            }
        #endregion

            #region Sync Team's equipment
            public async Task SyncTeamEquipmentAsync(CancellationToken ct = default)
            {
                var teams = await _context.Teams.ToListAsync(ct);

                foreach (var team in teams)
                {
                    try
                    {
                        if (_context.Entry(team).State == EntityState.Detached)
                            _context.Entry(team).State = EntityState.Unchanged;


                        List<Infrastructure.Integrations.DTO.SportsDbTeamEquipment> equipmentDtos;
                        try
                        {
                            equipmentDtos = await _client.GetTeamEquipmentAsync(team.ExternalId, ct);
                        }
                        catch (SportsDbApiException apiEx) when (apiEx.Message.Contains("No data found"))
                        {
                            _logger.LogInformation("No equipment data available for team ExternalId={ExternalTeamId} ({Team}) - skipping, this is expected for some teams.", team.ExternalId, team.Name);
                            equipmentDtos = new List<Infrastructure.Integrations.DTO.SportsDbTeamEquipment>();
                        }

                        foreach (var dto in equipmentDtos)
                        {
                            var equipment = await _context.TeamEquipment.FirstOrDefaultAsync(x => x.ExternalId == dto.IdEquipment, ct);

                            if (equipment is null)
                            {
                                equipment = new TeamEquipment { ExternalId = dto.IdEquipment };
                                _context.TeamEquipment.Add(equipment);
                            }

                            equipment.TeamId = team.Id;
                            equipment.Season = dto.StrSeason ?? string.Empty;
                            equipment.Equipment = dto.StrEquipment ?? string.Empty;
                            equipment.Type = dto.StrType ?? string.Empty;
                            equipment.Username = dto.StrUsername ?? string.Empty;
                            equipment.LastSyncedAt = DateTime.UtcNow;
                        }

                        await _context.SaveChangesAsync(ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sync dresova nije uspeo za tim ExternalId={ExternalTeamId}.", team.ExternalId);
                        _context.ChangeTracker.Clear();
                        await LogErrorAsync($"Failed to sync TeamEquipment for Team (ExternalId={team.ExternalId}, Name={team.Name}).", ex, ct);
                    }
                }
            }
        #endregion

            #region Sync Players
            public async Task SyncPlayersAsync(CancellationToken ct = default)
            {
                var teams = await _context.Teams.ToListAsync(ct);

                var teamIdCache = new Dictionary<string, long?>();

                async Task<long?> ResolveTeamIdAsync(string? externalTeamId)
                {
                    if (string.IsNullOrWhiteSpace(externalTeamId))
                        return null;

                    if (!teamIdCache.TryGetValue(externalTeamId, out var resolvedId))
                    {
                        resolvedId = await _context.Teams.Where(x => x.ExternalId == externalTeamId).Select(x => (long?)x.Id).FirstOrDefaultAsync(ct);
                        teamIdCache[externalTeamId] = resolvedId;
                    }

                    return resolvedId;
                }

                foreach (var team in teams)
                {
                    try
                    {
                        if (_context.Entry(team).State == EntityState.Detached)
                        {
                            _context.Entry(team).State = EntityState.Unchanged;
                        }

                        var playerDtos = await FetchListSafeAsync(() => _client.GetPlayersForTeamAsync(team.ExternalId, ct), $"players for team ExternalId={team.ExternalId} ({team.Name})", ct);

                        foreach (var listDto in playerDtos)
                        {
                            var player = await _context.Players.FirstOrDefaultAsync(x => x.ExternalId == listDto.IdPlayer, ct);
                            var isNewPlayer = player is null;

                            if (isNewPlayer)
                            {
                                player = new Player { ExternalId = listDto.IdPlayer };
                                _context.Players.Add(player);
                            }

                            if (isNewPlayer)
                            {
                                var detail = await FetchSingleSafeAsync(() => _client.GetPlayerDetailAsync(listDto.IdPlayer, ct), $"player detail ExternalId={listDto.IdPlayer} ({listDto.StrPlayer})", ct);

                                if (detail is not null)
                                {
                                    var (firstName, lastName) = SplitPlayerName(detail.StrPlayer, detail.StrLastName);

                                    player.FirstName = firstName;
                                    player.LastName = lastName;
                                    player.ImageUrl = detail.StrThumb ?? string.Empty;
                                    player.BirthLocation = detail.StrBirthLocation ?? string.Empty;
                                    player.Position = detail.StrPosition ?? string.Empty;
                                    player.Description = detail.StrDescriptionEN;
                                    player.Status = detail.StrStatus ?? string.Empty;
                                    player.Weight = detail.StrWeight ?? string.Empty;
                                    player.Height = detail.StrHeight ?? string.Empty;
                                    player.Worth = detail.StrWage;
                                    player.FacebookLink = detail.StrFacebook;
                                    player.InstagramLink = detail.StrInstagram;
                                    player.TwitterLink = detail.StrTwitter;
                                    player.YouTubeLink = detail.StrYoutube;

                                    if (DateOnly.TryParse(detail.DateBorn, out var birthDate))
                                    {
                                        player.BirthDate = birthDate;
                                    }

                                    if (DateOnly.TryParse(detail.DateDied, out var dateDied))
                                    {
                                        player.DateDied = dateDied;
                                    }

                                    // Primarni/sekundarni tim IZ AUTORITATIVNOG izvora (idTeam/idTeam2
                                    // sa pojedinacnog lookup/player odgovora) - NE iz "koji tim smo bas
                                    // sad obradjivali u petlji" (vidi objasnjenje u else-grani ispod).
                                    player.TeamId = await ResolveTeamIdAsync(detail.IdTeam) ?? team.Id;
                                    player.SecondaryTeamId = await ResolveTeamIdAsync(detail.IdTeam2);
                                }
                                else
                                {
                                    // lookup/player nije vratio nista (retko) - bar osnovno ime iz mrsavog odgovora
                                    var (firstName, lastName) = SplitPlayerName(listDto.StrPlayer, null);
                                    player.FirstName = firstName;
                                    player.LastName = lastName;
                                    player.Position = listDto.StrPosition ?? string.Empty;
                                    player.ImageUrl = listDto.StrThumb ?? string.Empty;

                                    if (DateOnly.TryParse(listDto.DateBorn, out var birthDate))
                                    {
                                        player.BirthDate = birthDate;
                                    }

                                    player.TeamId = team.Id;
                                }
                            }
                            else if (player.TeamId != team.Id && player.SecondaryTeamId != team.Id)
                            {
                                // POSTOJECI igrac se pojavio na rosteru tima koji NIJE ni njegov
                                // trenutno upisan primarni ni sekundarni tim - ili je ovo pravi
                                // transfer, ili je ovo drugi (klub/reprezentacija) tim koji tek
                                // sad otkrivamo. Bez ovog re-fetch-a, ranija verzija je NAIVNO
                                // prepisivala TeamId na "poslednji tim u petlji koji ga sadrzi" -
                                // potvrdjeno u praksi (Nikola Jokic je zavrsavao kao igrac SAMO
                                // "Serbia Basketball", brisuci Denver Nuggets, zavisno od redosleda
                                // sync-a timova). Pojedinacan detalj je jedini pouzdan izvor koji
                                // TACNO kaze koji je primarni a koji sekundarni.
                                var recheck = await FetchSingleSafeAsync(
                                    () => _client.GetPlayerDetailAsync(player.ExternalId, ct), $"player detail (re-check team membership) ExternalId={player.ExternalId} ({player.FirstName} {player.LastName})", ct);

                                if (recheck is not null)
                                {
                                    player.TeamId = await ResolveTeamIdAsync(recheck.IdTeam) ?? team.Id;
                                    player.SecondaryTeamId = await ResolveTeamIdAsync(recheck.IdTeam2);
                                }
                                else if (player.TeamId is null)
                                {
                                    player.TeamId = team.Id;
                                }
                                else
                                {
                                    player.SecondaryTeamId = team.Id;
                                }
                            }

                            player.LastSyncedAt = DateTime.UtcNow;
                        }

                        await _context.SaveChangesAsync(ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sync igraca nije uspeo za tim ExternalId={ExternalTeamId}.", team.ExternalId);
                        _context.ChangeTracker.Clear();
                        await LogErrorAsync($"Failed to sync Players for Team (ExternalId={team.ExternalId}, Name={team.Name}).", ex, ct);
                    }
                }
            }
        #endregion

            #region Sync Matches
            public async Task SyncMatchesAsync(CancellationToken ct = default)
            {
                await FixMissingFinishedStatusesAsync(ct);

                var leagues = await _context.Leagues.ToListAsync(ct);
                var channelIdCache = new Dictionary<string, long>();

                foreach (var league in leagues)
                {
                    try
                    {
                        if (_context.Entry(league).State == EntityState.Detached)
                        {
                            _context.Entry(league).State = EntityState.Unchanged;
                        }

                        var seasonMatches = new List<Infrastructure.Integrations.DTO.SportsDbMatch>();

                        if (!string.IsNullOrWhiteSpace(league.CurrentSeason))
                        {
                            seasonMatches = await GetMatchesSafeAsync(
                                () => _client.GetSeasonMatchesForLeagueAsync(league.ExternalId, league.CurrentSeason, ct), league, "season", ct);
                        }
                        else
                        {
                            _logger.LogInformation("League ExternalId={ExternalLeagueId} ({League}) has no CurrentSeason set - skipping season-wide match fetch.", league.ExternalId, league.Name);
                        }

                        var nextMatches = await GetMatchesSafeAsync(
                            () => _client.GetNextMatchesForLeagueAsync(league.ExternalId, ct), league, "upcoming", ct);

                        var previousMatches = await GetMatchesSafeAsync(
                            () => _client.GetPreviousMatchesForLeagueAsync(league.ExternalId, ct), league, "past", ct);

                        // Bitno: isti mec (isti idEvent) moze da se pojavi u VISE OD JEDNOG
                        // izvora (npr. i u "ova sezona" i u "naredni mecevi"). Bez ovog
                        // uklanjanja duplikata, UpsertMatchAsync bi drugi put pokusao da
                        // NAPRAVI nov red (jer prvi jos nije fizicki upisan u bazu unutar
                        // ISTOG SaveChangesAsync batch-a), sto puca na unique indeksu.
                        var allMatches = seasonMatches.Concat(nextMatches).Concat(previousMatches)
                            .GroupBy(m => m.IdEvent)
                            .Select(g => g.First())
                            .ToList();

                        var newMatches = new List<Match>();

                        foreach (var dto in allMatches)
                        {
                            var created = await UpsertMatchAsync(dto, ct);
                            if (created is not null)
                                newMatches.Add(created);
                        }

                        await _context.SaveChangesAsync(ct);

                        // TV prenosi SAMO za mecove koji su upravo prvi put upisani (treba nam njihov Id, pa
                        // tek posle SaveChanges). Neuspeh TV sync-a NE sme da obori sync meceva.
                        foreach (var newMatch in newMatches)
                        {
                            try
                            {
                                await SyncTvForMatchAsync(newMatch.Id, newMatch.ExternalId, newMatch.Name, newMatch.EventDate, channelIdCache, ct);
                            }
                            catch (Exception tvEx)
                            {
                                _logger.LogError(tvEx, "TV sync novog meca nije uspeo (ExternalId={ExternalEventId}).", newMatch.ExternalId);
                                _context.ChangeTracker.Clear();
                                await LogErrorAsync($"Failed to sync TV broadcasts for NEW Match (ExternalId={newMatch.ExternalId}, Name={newMatch.Name}).", tvEx, ct);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sync meceva nije uspeo za ligu ExternalId={ExternalLeagueId}.", league.ExternalId);
                        _context.ChangeTracker.Clear();
                        await LogErrorAsync($"Failed to sync Matches for League (ExternalId={league.ExternalId}, Name={league.Name}).", ex, ct);
                    }
                }
            }
            #endregion

            /// Poziva dati fetch delegat (next ili previous mecevi) i tretira "No data found"
            /// kao normalnu, ocekivanu situaciju (ne gresku) - vraca praznu listu umesto da baci dalje.
            private async Task<List<Infrastructure.Integrations.DTO.SportsDbMatch>> GetMatchesSafeAsync(
                Func<Task<List<Infrastructure.Integrations.DTO.SportsDbMatch>>> fetch,
                League league,
                string kind,
                CancellationToken ct)
            {
                try
                {
                    return await fetch();
                }
                catch (SportsDbApiException apiEx) when (apiEx.Message.Contains("No data found"))
                {
                    _logger.LogInformation(
                        "No {Kind} matches available for league ExternalId={ExternalLeagueId} ({League}) - skipping, this is expected for some leagues.",
                        kind, league.ExternalId, league.Name);
                    return new List<Infrastructure.Integrations.DTO.SportsDbMatch>();
                }
            }

            #region Upsert match
            /// Vraca Match SAMO ako je ovim pozivom NAPRAVLJEN nov red (null ako je azuriran postojeci ili preskocen).
            private async Task<Match?> UpsertMatchAsync(Infrastructure.Integrations.DTO.SportsDbMatch dto, CancellationToken ct)
            {
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.ExternalId == dto.IdLeague, ct);
                if (league is null)
                {
                    _logger.LogWarning("League ExternalId={ExternalLeagueId} not found for match '{Match}' - skipping.", dto.IdLeague, dto.StrEvent);
                    return null;
                }

                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.ExternalId == dto.IdHomeTeam, ct);
                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.ExternalId == dto.IdAwayTeam, ct);
                if (homeTeam is null || awayTeam is null)
                {
                    _logger.LogWarning("HomeTeam/AwayTeam not found for match '{Match}' (ExternalId={ExternalEventId}) - skipping.", dto.StrEvent, dto.IdEvent);
                    return null;
                }

                var eventDate = ParseEventDate(dto);
                if (eventDate is null)
                {
                    _logger.LogWarning("Could not parse event date for match '{Match}' (ExternalId={ExternalEventId}) - skipping.", dto.StrEvent, dto.IdEvent);
                    return null;
                }

                var match = await _context.Matches.FirstOrDefaultAsync(x => x.ExternalId == dto.IdEvent, ct);

                var isNew = match is null;
                if (match is null)
                {
                    match = new Match { ExternalId = dto.IdEvent };
                    _context.Matches.Add(match);
                }

                match.Name = dto.StrEvent;
                match.EventDate = eventDate.Value;
                match.LeagueId = league.Id;
                match.HomeTeamId = homeTeam.Id;
                match.AwayTeamId = awayTeam.Id;
                match.Season = dto.StrSeason ?? string.Empty;
                match.Round = dto.IntRound ?? string.Empty;
                match.Status = dto.StrStatus ?? string.Empty;
                match.Progress = dto.StrProgress;
                match.LastSyncedAt = DateTime.UtcNow;

                if (int.TryParse(dto.IntHomeScore, out var homeScore))
                {
                    match.HomeTeamScore = homeScore;
                }

                if (int.TryParse(dto.IntAwayScore, out var awayScore))
                {
                    match.AwayTeamScore = awayScore;
                }

                if (!string.IsNullOrWhiteSpace(dto.IdVenue))
                {
                    var venue = await _context.Venues.FirstOrDefaultAsync(x => x.ExternalId == dto.IdVenue, ct);
                    match.VenueId = venue?.Id; // null ako nije pronadjen - VenueId je opcion na Match
                }

                return isNew ? match : null;
            }
            #endregion     

            #region Sync Historical Matches
        public async Task SyncHistoricalMatchesAsync(CancellationToken ct = default)
                {
                    var leagues = await _context.Leagues.ToListAsync(ct);

                    foreach (var league in leagues)
                    {
                        try
                        {
                            if (_context.Entry(league).State == EntityState.Detached)
                            {
                                _context.Entry(league).State = EntityState.Unchanged;
                            }

                            if (string.IsNullOrWhiteSpace(league.CurrentSeason))
                            {
                                _logger.LogInformation("League ExternalId={ExternalLeagueId} ({League}) has no CurrentSeason set - cannot generate historical seasons, skipping.", league.ExternalId, league.Name);
                                continue;
                            }

                            const int maxConsecutiveMisses = 3;
                            var consecutiveMisses = 0;
                            var seasonsFetched = 0;

                            foreach (var season in GenerateSeasonsGoingBack(league.CurrentSeason))
                            {
                                List<Infrastructure.Integrations.DTO.SportsDbMatch> seasonMatches;

                                try
                                {
                                    seasonMatches = await _client.GetSeasonMatchesForLeagueAsync(league.ExternalId, season, ct);
                                }
                                catch (SportsDbApiException apiEx) when (apiEx.Message.Contains("No data found"))
                                {
                                    consecutiveMisses++;

                                    if (consecutiveMisses >= maxConsecutiveMisses)
                                    {
                                        _logger.LogInformation(
                                            "League ExternalId={ExternalLeagueId} ({League}): {Misses} consecutive seasons with no data - stopping historical backfill at season {Season}.",
                                            league.ExternalId, league.Name, consecutiveMisses, season);
                                        break;
                                    }

                                    continue;
                                }

                                consecutiveMisses = 0; // nasli smo podatke - resetuj brojac promasaja
                                seasonsFetched++;

                                foreach (var dto in seasonMatches)
                                {
                                    await UpsertMatchAsync(dto, ct);
                                }

                                await _context.SaveChangesAsync(ct);
                            }

                            _logger.LogInformation("Historical match backfill done for league ExternalId={ExternalLeagueId} ({League}) - {Seasons} seasons found.", league.ExternalId, league.Name, seasonsFetched);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Historical match sync nije uspeo za ligu ExternalId={ExternalLeagueId}.", league.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed historical match sync for League (ExternalId={league.ExternalId}, Name={league.Name}).", ex, ct);
                        }
                    }
                }
                #endregion


        

            #region Sync Match Stats And Lineup
                public async Task SyncMatchDetailsAsync(CancellationToken ct = default)
                {
                    // Ispravi stare mecove sa rezultatom a bez statusa PRE nego trazimo kandidate -
                    // inace bi ostali nevidljivi za upit ispod (koji trazi Status u FinishedMatchStatuses).
                    await FixMissingFinishedStatusesAsync(ct);

                    var matchesNeedingDetails = await _context.Matches
                        .Where(m => FinishedMatchStatuses.Contains(m.Status))
                        .Where(m => m.DetailsSyncedAt == null)
                        .ToListAsync(ct);

                    _logger.LogInformation("Found {Count} finished matches without statistics - fetching details.", matchesNeedingDetails.Count);

                    foreach (var match in matchesNeedingDetails)
                    {
                        try
                        {
                            if (_context.Entry(match).State == EntityState.Detached)
                            {
                                _context.Entry(match).State = EntityState.Unchanged;
                            }

                            List<Infrastructure.Integrations.DTO.SportsDbEventStatistic> statDtos;
                            try
                            {
                                statDtos = await _client.GetEventStatisticsAsync(match.ExternalId, ct);
                            }
                            catch (SportsDbApiException apiEx) when (apiEx.Message.Contains("No data found"))
                            {
                                _logger.LogInformation("No statistics available for match ExternalId={ExternalEventId} ({Match}) - skipping, this is expected for some matches.", match.ExternalId, match.Name);
                                statDtos = new List<Infrastructure.Integrations.DTO.SportsDbEventStatistic>();
                            }

                            // Statistika (ostaje ovde, nezavisno)
                            foreach (var statDto in statDtos)
                            {
                                var stat = await _context.EventStatistics.FirstOrDefaultAsync(x => x.EventId == match.Id && x.Stat == statDto.StrStat, ct);

                                if (stat is null)
                                {
                                    stat = new EventStatistic { ExternalId = statDto.IdStatistic, EventId = match.Id, Stat = statDto.StrStat };
                                    _context.EventStatistics.Add(stat);
                                }

                                stat.EventName = statDto.StrEvent ?? match.Name;
                                stat.HomeScore = double.TryParse(statDto.IntHome, out var homeVal) ? homeVal : 0;
                                stat.AwayScore = double.TryParse(statDto.IntAway, out var awayVal) ? awayVal : 0;
                                stat.LastSyncedAt = DateTime.UtcNow;
                            }

                            // Postava - kroz deljenu metodu (ista logika koju koriste live tracker i pre-mec provera)
                            await UpsertLineupsForMatchAsync(match, ct);

                            await _context.SaveChangesAsync(ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Sync detalja meca nije uspeo za mec ExternalId={ExternalEventId}.", match.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to sync Match details (Stats+Lineup) for Match (ExternalId={match.ExternalId}, Name={match.Name}).", ex, ct);
                            continue; // NE postavljamo DetailsSyncedAt - probacemo ponovo sledeci put
                        }

                        // Tek OVDE, POSLE uspesnog zavrsetka bez izuzetka (i statistika i postava),
                        // oznacavamo mec kao gotov - ako je bilo koji deo pukao, kod iznad vec je
                        // uradio "continue" i preskocio ovu liniju.
                        match.DetailsSyncedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync(ct);
                    }
                }
            #endregion

            #region Sync player's career details
                public async Task SyncPlayerStatisticsAsync(CancellationToken ct = default)
                {
                    var cutoff = DateTime.UtcNow.AddMonths(-11);
                    var players = await _context.Players
                        .Where(p => p.StatisticsSyncedAt == null || p.StatisticsSyncedAt < cutoff)
                        .ToListAsync(ct);

                    _logger.LogInformation("SyncPlayerStatisticsAsync: {Count} igraca treba osvezavanje statistike.", players.Count);

                    foreach (var player in players)
                    {
                        try
                        {
                            if (_context.Entry(player).State == EntityState.Detached)
                            {
                                _context.Entry(player).State = EntityState.Unchanged;
                            }

                            var statDtos = await FetchListSafeAsync(
                                () => _client.GetPlayerStatisticsAsync(player.ExternalId, ct), $"statistics for player ExternalId={player.ExternalId} ({player.FirstName} {player.LastName})", ct);

                            // Ucitaj SVE postojece stavke JEDNIM upitom (ne po jednoj za svaku stavku),
                            // i kesiraj Team/League pretrage unutar ove iteracije - isti tim/liga se
                            // cesto ponavlja kroz vise sezona istog igraca.
                            var existingStats = await _context.PlayerStatistics
                                .Where(x => x.PlayerId == player.Id)
                                .ToDictionaryAsync(x => x.ExternalId, ct);

                            var teamCache = new Dictionary<string, long?>();
                            var leagueCache = new Dictionary<string, long?>();

                            foreach (var dto in statDtos)
                            {
                                if (!existingStats.TryGetValue(dto.Id, out var stat))
                                {
                                    stat = new PlayerStatistic { ExternalId = dto.Id, PlayerId = player.Id };
                                    _context.PlayerStatistics.Add(stat);
                                    existingStats[dto.Id] = stat;
                                }

                                stat.Statistic = dto.StrStatistic;
                                stat.Value = dto.StrValue;
                                stat.Season = dto.StrSeason;
                                stat.LastSyncedAt = DateTime.UtcNow;

                                if (!string.IsNullOrWhiteSpace(dto.IdTeam))
                                {
                                    if (!teamCache.TryGetValue(dto.IdTeam, out var teamId))
                                    {
                                        var statTeam = await _context.Teams.FirstOrDefaultAsync(x => x.ExternalId == dto.IdTeam, ct);
                                        teamId = statTeam?.Id;
                                        teamCache[dto.IdTeam] = teamId;
                                    }
                                    stat.TeamId = teamId;
                                }

                                if (!string.IsNullOrWhiteSpace(dto.IdLeague))
                                {
                                    if (!leagueCache.TryGetValue(dto.IdLeague, out var leagueId))
                                    {
                                        var statLeague = await _context.Leagues.FirstOrDefaultAsync(x => x.ExternalId == dto.IdLeague, ct);
                                        leagueId = statLeague?.Id;
                                        leagueCache[dto.IdLeague] = leagueId;
                                    }
                                    stat.LeagueId = leagueId;
                                }
                            }

                            player.StatisticsSyncedAt = DateTime.UtcNow;
                            await _context.SaveChangesAsync(ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Sync statistike nije uspeo za igraca ExternalId={ExternalPlayerId}.", player.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to sync PlayerStatistic for Player (ExternalId={player.ExternalId}, Name={player.FirstName} {player.LastName}).", ex, ct);
                        }
                    }
                }

                public async Task SyncPlayerFormerTeamsAsync(CancellationToken ct = default)
                {
                    var cutoff = DateTime.UtcNow.AddMonths(-6);
                    var players = await _context.Players
                        .Where(p => p.FormerTeamsSyncedAt == null || p.FormerTeamsSyncedAt < cutoff)
                        .ToListAsync(ct);

                    _logger.LogInformation("SyncPlayerFormerTeamsAsync: {Count} igraca treba osvezavanje transfera.", players.Count);

                    foreach (var player in players)
                    {
                        try
                        {
                            if (_context.Entry(player).State == EntityState.Detached)
                            {
                                _context.Entry(player).State = EntityState.Unchanged;
                            }

                            var formerTeamDtos = await FetchListSafeAsync(
                                () => _client.GetPlayerFormerTeamsAsync(player.ExternalId, ct), $"former teams for player ExternalId={player.ExternalId} ({player.FirstName} {player.LastName})", ct);

                            var existingEntries = await _context.PlayerFormerTeams
                                .Where(x => x.PlayerId == player.Id)
                                .ToDictionaryAsync(x => x.ExternalId, ct);

                            var teamCache = new Dictionary<string, Team?>();

                            foreach (var dto in formerTeamDtos)
                            {
                                if (!teamCache.TryGetValue(dto.IdFormerTeam, out var formerTeam))
                                {
                                    formerTeam = await _context.Teams.FirstOrDefaultAsync(x => x.ExternalId == dto.IdFormerTeam, ct);
                                    teamCache[dto.IdFormerTeam] = formerTeam;
                                }

                                if (formerTeam is null)
                                {
                                    _logger.LogWarning("Former team ExternalId={ExternalTeamId} not found for player ExternalId={ExternalPlayerId} - skipping this entry.", dto.IdFormerTeam, player.ExternalId);
                                    continue;
                                }

                                if (!existingEntries.TryGetValue(dto.Id, out var entry))
                                {
                                    entry = new PlayerFormerTeam { ExternalId = dto.Id, PlayerId = player.Id };
                                    _context.PlayerFormerTeams.Add(entry);
                                    existingEntries[dto.Id] = entry;
                                }

                                entry.FormerTeamId = formerTeam.Id;
                                entry.MoveType = dto.StrMoveType;
                                entry.BadgeUrl = dto.StrBadge;
                                entry.Joined = dto.StrJoined;
                                entry.Departed = dto.StrDeparted;
                                entry.LastSyncedAt = DateTime.UtcNow;

                                if (int.TryParse(dto.IntAppearances, out var appearances))
                                {
                                    entry.Appearances = appearances;
                                }

                                if (int.TryParse(dto.IntGoals, out var goals))
                                {
                                    entry.Goals = goals;
                                }
                            }

                            player.FormerTeamsSyncedAt = DateTime.UtcNow;
                            await _context.SaveChangesAsync(ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Sync transfera nije uspeo za igraca ExternalId={ExternalPlayerId}.", player.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to sync PlayerFormerTeam for Player (ExternalId={player.ExternalId}, Name={player.FirstName} {player.LastName}).", ex, ct);
                        }
                    }
                }

                public async Task SyncPlayerHonoursAsync(CancellationToken ct = default)
                {
                    var cutoff = DateTime.UtcNow.AddMonths(-11);
                    var players = await _context.Players
                        .Where(p => p.HonoursSyncedAt == null || p.HonoursSyncedAt < cutoff)
                        .ToListAsync(ct);

                    _logger.LogInformation("SyncPlayerHonoursAsync: {Count} igraca treba osvezavanje trofeja.", players.Count);

                    foreach (var player in players)
                    {
                        try
                        {
                            if (_context.Entry(player).State == EntityState.Detached)
                            {
                                _context.Entry(player).State = EntityState.Unchanged;
                            }

                            var honourDtos = await FetchListSafeAsync(
                                () => _client.GetPlayerHonoursAsync(player.ExternalId, ct), $"honours for player ExternalId={player.ExternalId} ({player.FirstName} {player.LastName})", ct);

                            var existingHonours = await _context.PlayerHonours
                                .Where(x => x.PlayerId == player.Id)
                                .ToDictionaryAsync(x => x.ExternalId, ct);

                            var teamCache = new Dictionary<string, Team?>();
                            var leagueCache = new Dictionary<string, long?>();

                            foreach (var dto in honourDtos)
                            {
                                if (!teamCache.TryGetValue(dto.IdTeam, out var honourTeam))
                                {
                                    honourTeam = await _context.Teams.FirstOrDefaultAsync(x => x.ExternalId == dto.IdTeam, ct);
                                    teamCache[dto.IdTeam] = honourTeam;
                                }

                                if (honourTeam is null)
                                {
                                    _logger.LogWarning("Team ExternalId={ExternalTeamId} not found for honour entry of player ExternalId={ExternalPlayerId} - skipping this entry.", dto.IdTeam, player.ExternalId);
                                    continue;
                                }

                                if (!existingHonours.TryGetValue(dto.Id, out var honour))
                                {
                                    honour = new PlayerHonour { ExternalId = dto.Id, PlayerId = player.Id };
                                    _context.PlayerHonours.Add(honour);
                                    existingHonours[dto.Id] = honour;
                                }

                                honour.TeamId = honourTeam.Id;
                                honour.HonourId = dto.IdHonour;
                                honour.HonourName = dto.StrHonour;
                                honour.HonourLogoUrl = dto.StrHonourLogo;
                                honour.HonourTrophyUrl = dto.StrHonourTrophy;
                                honour.Season = dto.StrSeason;
                                honour.LastSyncedAt = DateTime.UtcNow;

                                if (!string.IsNullOrWhiteSpace(dto.IdLeague))
                                {
                                    if (!leagueCache.TryGetValue(dto.IdLeague, out var leagueId))
                                    {
                                        var honourLeague = await _context.Leagues.FirstOrDefaultAsync(x => x.ExternalId == dto.IdLeague, ct);
                                        leagueId = honourLeague?.Id;
                                        leagueCache[dto.IdLeague] = leagueId;
                                    }
                                    honour.LeagueId = leagueId;
                                }
                            }

                            player.HonoursSyncedAt = DateTime.UtcNow;
                            await _context.SaveChangesAsync(ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Sync trofeja nije uspeo za igraca ExternalId={ExternalPlayerId}.", player.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to sync PlayerHonour for Player (ExternalId={player.ExternalId}, Name={player.FirstName} {player.LastName}).", ex, ct);
                        }
                    }
                }

                public async Task SyncPlayerMilestonesAsync(CancellationToken ct = default)
                {
                    var cutoff = DateTime.UtcNow.AddMonths(-11);
                    var players = await _context.Players
                        .Where(p => p.MilestonesSyncedAt == null || p.MilestonesSyncedAt < cutoff)
                        .ToListAsync(ct);

                    _logger.LogInformation("SyncPlayerMilestonesAsync: {Count} igraca treba osvezavanje milestone-a.", players.Count);

                    foreach (var player in players)
                    {
                        try
                        {
                            if (_context.Entry(player).State == EntityState.Detached)
                            {
                                _context.Entry(player).State = EntityState.Unchanged;
                            }

                            var milestoneDtos = await FetchListSafeAsync(
                                () => _client.GetPlayerMilestonesAsync(player.ExternalId, ct), $"milestones for player ExternalId={player.ExternalId} ({player.FirstName} {player.LastName})", ct);

                            var existingMilestones = await _context.PlayerMilestones
                                .Where(x => x.PlayerId == player.Id)
                                .ToDictionaryAsync(x => x.ExternalId, ct);

                            var teamCache = new Dictionary<string, Team?>();

                            foreach (var dto in milestoneDtos)
                            {
                                if (!teamCache.TryGetValue(dto.IdTeam, out var milestoneTeam))
                                {
                                    milestoneTeam = await _context.Teams.FirstOrDefaultAsync(x => x.ExternalId == dto.IdTeam, ct);
                                    teamCache[dto.IdTeam] = milestoneTeam;
                                }

                                if (milestoneTeam is null)
                                {
                                    _logger.LogWarning("Team ExternalId={ExternalTeamId} not found for milestone entry of player ExternalId={ExternalPlayerId} - skipping this entry.", dto.IdTeam, player.ExternalId);
                                    continue;
                                }

                                if (!existingMilestones.TryGetValue(dto.Id, out var milestone))
                                {
                                    milestone = new PlayerMilestone { ExternalId = dto.Id, PlayerId = player.Id };
                                    _context.PlayerMilestones.Add(milestone);
                                    existingMilestones[dto.Id] = milestone;
                                }

                                milestone.TeamId = milestoneTeam.Id;
                                milestone.MilestoneId = dto.IdMilestone;
                                milestone.MilestoneName = dto.StrMilestone;
                                milestone.MilestoneLogoUrl = dto.StrMilestoneLogo;
                                milestone.LastSyncedAt = DateTime.UtcNow;

                                if (DateOnly.TryParse(dto.DateMilestone, out var dateAchieved))
                                {
                                    milestone.DateAchieved = dateAchieved;
                                }
                            }

                            player.MilestonesSyncedAt = DateTime.UtcNow;
                            await _context.SaveChangesAsync(ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Sync milestone-a nije uspeo za igraca ExternalId={ExternalPlayerId}.", player.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to sync PlayerMilestone for Player (ExternalId={player.ExternalId}, Name={player.FirstName} {player.LastName}).", ex, ct);
                        }
                    }
                }

                /// <summary>
                /// Obogacuje igrace sa MINIMALNIM podacima - dva moguca izvora:
                /// 1) TeamId == null - napravljeni iz postave meca (UpsertLineupsForMatchAsync)
                /// 2) Status/Height prazni - napravljeni u SyncPlayersAsync kad lookup/player/{id}
                ///    nije vratio nista za NOVOG igraca (redak slucaj, TeamId JESTE postavljen)
                /// Bezbedno - trazi se po TACNOM, jedinstvenom ExternalId, ne po imenu, pa nema
                /// rizika od zabune sa istoimenim igracima. TeamId se NE dira ako je vec null -
                /// ne znamo njihov TRENUTNI tim za istorijske (postava) slucajeve.
                /// </summary>
                public async Task SyncStubPlayerProfilesAsync(CancellationToken ct = default)
                {
                    var stubPlayers = await _context.Players
                        .Where(p => p.TeamId == null
                                 || ((p.Status == null || p.Status == string.Empty)
                                     && (p.Height == null || p.Height == string.Empty)))
                        .ToListAsync(ct);

                    _logger.LogInformation("SyncStubPlayerProfilesAsync: {Count} 'stub' igraca za obogacivanje.", stubPlayers.Count);

                    foreach (var player in stubPlayers)
                    {
                        try
                        {
                            if (_context.Entry(player).State == EntityState.Detached)
                            {
                                _context.Entry(player).State = EntityState.Unchanged;
                            }

                            var detail = await FetchSingleSafeAsync(
                                () => _client.GetPlayerDetailAsync(player.ExternalId, ct), $"stub player detail ExternalId={player.ExternalId} ({player.FirstName})", ct);

                            if (detail is null)
                            {
                                _logger.LogInformation("No detail available for stub player ExternalId={ExternalPlayerId} - ostaje minimalan.", player.ExternalId);
                                continue;
                            }

                            var (firstName, lastName) = SplitPlayerName(detail.StrPlayer, detail.StrLastName);

                            player.FirstName = firstName;
                            player.LastName = lastName;
                            player.ImageUrl = detail.StrThumb ?? string.Empty;
                            player.BirthLocation = detail.StrBirthLocation ?? string.Empty;
                            player.Position = detail.StrPosition ?? player.Position;
                            player.Description = detail.StrDescriptionEN;
                            player.Status = detail.StrStatus ?? string.Empty;
                            player.Weight = detail.StrWeight ?? string.Empty;
                            player.Height = detail.StrHeight ?? string.Empty;
                            player.Worth = detail.StrWage;
                            player.FacebookLink = detail.StrFacebook;
                            player.InstagramLink = detail.StrInstagram;
                            player.TwitterLink = detail.StrTwitter;
                            player.YouTubeLink = detail.StrYoutube;

                            if (DateOnly.TryParse(detail.DateBorn, out var birthDate))
                            {
                                player.BirthDate = birthDate;
                            }

                            if (DateOnly.TryParse(detail.DateDied, out var dateDied))
                            {
                                player.DateDied = dateDied;
                            }

                            // TeamId NAMERNO ostaje null - vidi napomenu iznad.
                            await _context.SaveChangesAsync(ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Obogacivanje stub igraca nije uspelo za ExternalId={ExternalPlayerId}.", player.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to enrich stub Player (ExternalId={player.ExternalId}).", ex, ct);
                        }
                    }
                }

                /// <summary>
                /// Pogodnost za rucno "uradi sve" - poziva sve 4 metode redom. Automatski rasporedi
                /// (BackgroundService-i) pozivaju POJEDINACNE metode, ne ovu, jer svaka sad ima svoj ritam.
                /// </summary>
                public async Task SyncPlayerCareerDataAsync(CancellationToken ct = default)
                {
                    await SyncPlayerStatisticsAsync(ct);
                    await SyncPlayerFormerTeamsAsync(ct);
                    await SyncPlayerHonoursAsync(ct);
                    await SyncPlayerMilestonesAsync(ct);
                }
                #endregion

            #region Sync live match statistics
                private static readonly TimeSpan LiveMatchWindow = TimeSpan.FromHours(3);
                private static readonly string[] FinishedMatchStatuses = { "FT", "AET", "PEN" };

                // TheSportsDB V2 livescore (brzo osvezavanje) je zvanicno podrzan SAMO za ovih 5
                // sportova - ostali sportovi/lige i dalje dobijaju status/rezultat kroz redovan
                // 6h SyncMatchesAsync ciklus, samo ne na 2 min.
                private static readonly string[] LiveTrackedSports = { "Soccer", "American Football", "Basketball", "Baseball", "Ice Hockey" };

                /// <summary>
                /// Prati status/rezultat, statistiku I postavu (hvata izmene/rezerve) meceva u
                /// "prozoru" [EventDate, EventDate + 3h] - SAMO za sportove koje SportsDB V2 nudi
                /// kao brzo osvezavane. DVA uslova sprecavaju nepotreban rad: (1) vremenski prozor,
                /// (2) cim Status postane zavrsen, mec ODMAH ispada iz sledeceg ciklusa.
                /// Namenjeno pozivanju na 2 min iz pozadinskog servisa.
                /// </summary>
                public async Task SyncLiveMatchStatisticsAsync(CancellationToken ct = default)
                {
                    var now = DateTime.UtcNow;
                    var windowStart = now - LiveMatchWindow;

                    var liveMatches = await _context.Matches
                        .Include(m => m.League).ThenInclude(l => l.Sport)
                        .Include(m => m.HomeTeam)
                        .Include(m => m.AwayTeam)
                        .Where(m => m.EventDate <= now && m.EventDate >= windowStart)
                        .Where(m => !FinishedMatchStatuses.Contains(m.Status))
                        .Where(m => LiveTrackedSports.Contains(m.League.Sport.Name))
                        .ToListAsync(ct);

                    if (liveMatches.Count == 0)
                        return; 

                    
                    var progressBySport = new Dictionary<string, Dictionary<string, string?>>();

                    foreach (var sportName in liveMatches.Select(m => m.League.Sport.Name).Distinct())
                    {
                        var segment = ToLiveScoreSportSegment(sportName);
                        var liveScores = await FetchListSafeAsync(
                            () => _client.GetLiveScoresAsync(segment, ct), $"live scores for sport '{sportName}' (segment '{segment}')", ct);

                        progressBySport[sportName] = liveScores
                            .Where(ls => !string.IsNullOrWhiteSpace(ls.IdEvent))
                            .GroupBy(ls => ls.IdEvent)
                            .ToDictionary(g => g.Key, g => g.First().StrProgress);
                    }

                    
                    var leaguesToRefreshStandings = new HashSet<long>();

                    foreach (var match in liveMatches)
                    {
                        try
                        {
                            if (_context.Entry(match).State == EntityState.Detached)
                                _context.Entry(match).State = EntityState.Unchanged;
                            

                            var wasFinished = FinishedMatchStatuses.Contains(match.Status);
                            var oldHomeScore = match.HomeTeamScore;
                            var oldAwayScore = match.AwayTeamScore;

                            // 1. Status/rezultat - OVO je kljucno da znamo kad je mec zavrsen
                            var eventDetail = await FetchSingleSafeAsync(
                                () => _client.GetEventDetailAsync(match.ExternalId, ct), $"live event detail ExternalId={match.ExternalId} ({match.Name})", ct);

                            if (eventDetail is not null)
                            {
                                match.Status = eventDetail.StrStatus ?? match.Status;
                                match.Progress = eventDetail.StrProgress;

                                if (int.TryParse(eventDetail.IntHomeScore, out var homeScore))
                                {
                                    match.HomeTeamScore = homeScore;
                                }

                                if (int.TryParse(eventDetail.IntAwayScore, out var awayScore))
                                {
                                    match.AwayTeamScore = awayScore;
                                }

                                match.LastSyncedAt = DateTime.UtcNow;
                            }

                            // Dopuni Progress iz livescore/{sport} (eventDetail.StrProgress iznad
                            // je uvek prazan) - ako mec vise nije u tom trenutku na listi uzivo
                            // rezultata (npr. bas presao u prekid), ostaje ono sto je vec upisano.
                            if (progressBySport.TryGetValue(match.League.Sport.Name, out var progressByEvent)
                                && progressByEvent.TryGetValue(match.ExternalId, out var liveProgress)
                                && !string.IsNullOrWhiteSpace(liveProgress))
                            {
                                match.Progress = liveProgress;

                                // livescore je azurniji od lookup/event: ako vec vraca minut, mec JE u toku
                                // iako lookup jos javlja "NS" - taj zastareli status se ne cuva (klijent
                                // tada prikazuje samo minut, a ne "NS - 2'").
                                if (string.Equals(match.Status, "NS", StringComparison.OrdinalIgnoreCase))
                                    match.Status = string.Empty;
                            }

                            // 2. Statistika (sutevi, posed, itd.)
                            var statDtos = await FetchListSafeAsync(
                                () => _client.GetEventStatisticsAsync(match.ExternalId, ct), $"live statistics for match ExternalId={match.ExternalId} ({match.Name})", ct);

                            foreach (var statDto in statDtos)
                            {
                                var stat = await _context.EventStatistics.FirstOrDefaultAsync(x => x.EventId == match.Id && x.Stat == statDto.StrStat, ct);

                                if (stat is null)
                                {
                                    stat = new EventStatistic { ExternalId = statDto.IdStatistic, EventId = match.Id, Stat = statDto.StrStat };
                                    _context.EventStatistics.Add(stat);
                                }

                                stat.EventName = statDto.StrEvent ?? match.Name;
                                stat.HomeScore = double.TryParse(statDto.IntHome, out var homeVal) ? homeVal : 0;
                                stat.AwayScore = double.TryParse(statDto.IntAway, out var awayVal) ? awayVal : 0;
                                stat.LastSyncedAt = DateTime.UtcNow;
                            }

                            await _context.SaveChangesAsync(ct);

                            // Gol - SAMO fudbal (korisnicki zahtev), i SAMO ako je skor stvarno
                            // porastao (ne pri svakom sync ciklusu) - push poziv nikad ne sme da
                            // obori sync, zato svoj-svoj try/catch koji samo loguje.
                            if (match.League.Sport.Name == "Soccer")
                            {
                                try
                                {
                                    if (match.HomeTeamScore > oldHomeScore)
                                        await _matchNotificationService.SendGoalNotificationAsync(match, isHomeGoal: true, ct);

                                    if (match.AwayTeamScore > oldAwayScore)
                                        await _matchNotificationService.SendGoalNotificationAsync(match, isHomeGoal: false, ct);
                                }
                                catch (Exception notifyEx)
                                {
                                    _logger.LogError(notifyEx, "Slanje gol-notifikacije nije uspelo za Match Id={MatchId}.", match.Id);
                                }
                            }

                            // 3. Postava - hvata izmene/rezerve tokom meca
                            await UpsertLineupsForMatchAsync(match, ct);

                            // 4. Mec je UPRAVO zavrsio (proceneno iz eventDetail.StrStatus iznad,
                            // ne iz Season stringa ili sata) - zapamti ligu za osvezavanje tabele.
                            if (!wasFinished && FinishedMatchStatuses.Contains(match.Status))
                            {
                                leaguesToRefreshStandings.Add(match.LeagueId);

                                try
                                {
                                    await _matchNotificationService.SendFinalScoreNotificationAsync(match, ct);
                                }
                                catch (Exception notifyEx)
                                {
                                    _logger.LogError(notifyEx, "Slanje notifikacije o krajnjem rezultatu nije uspelo za Match Id={MatchId}.", match.Id);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Live sync statistike nije uspeo za mec ExternalId={ExternalEventId}.", match.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to sync live statistics for Match (ExternalId={match.ExternalId}, Name={match.Name}).", ex, ct);
                        }
                    }

                    foreach (var leagueId in leaguesToRefreshStandings)
                    {
                        try
                        {
                            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId, ct);

                            if (league is not null)
                            {
                                await SyncStandingsForLeagueAsync(league, ct);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Standings refresh (posle zavrsenog meca) nije uspeo za League Id={LeagueId}.", leagueId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to refresh standings after a finished match for LeagueId={leagueId}.", ex, ct);
                        }
                    }
                }
                #endregion

            #region Sync standings
                /// <summary>
                /// Tabela lige (Standings) za JEDNU ligu, njena trenutna sezona - upsert po
                /// ExternalId (idStanding). Poziva se automatski (iz SyncLiveMatchStatisticsAsync,
                /// cim mec u toj ligi zavrsi) i rucno (iz SyncStandingsAsync/admin endpointa).
                /// </summary>
                public async Task SyncStandingsForLeagueAsync(League league, CancellationToken ct = default)
                {
                    if (string.IsNullOrWhiteSpace(league.CurrentSeason))
                    {
                        _logger.LogInformation("League ExternalId={ExternalLeagueId} ({League}) has no CurrentSeason set - skipping standings sync.", league.ExternalId, league.Name);
                        return;
                    }

                    var standingDtos = await FetchListSafeAsync(
                        () => _client.GetLeagueStandingsAsync(league.ExternalId, league.CurrentSeason, ct),
                        $"standings for league ExternalId={league.ExternalId} ({league.Name})", ct);

                    await UpsertStandingsAsync(league, standingDtos, league.CurrentSeason, ct);
                }

                /// <summary>
                /// Deljena upsert logika (po ExternalId/idStanding) - koriste je i
                /// SyncStandingsForLeagueAsync (tekuca sezona) i SyncHistoricalStandingsAsync
                /// (svaka prosla sezona posebno, otud eksplicitan "season" umesto
                /// league.CurrentSeason kao fallback-a).
                /// </summary>
                private async Task UpsertStandingsAsync(League league, List<Infrastructure.Integrations.DTO.SportsDbStanding> standingDtos, string season, CancellationToken ct)
                {
                    var teamCache = new Dictionary<string, long?>();

                    foreach (var dto in standingDtos)
                    {
                        if (!teamCache.TryGetValue(dto.IdTeam, out var teamId))
                        {
                            teamId = await _context.Teams.Where(x => x.ExternalId == dto.IdTeam).Select(x => (long?)x.Id).FirstOrDefaultAsync(ct);
                            teamCache[dto.IdTeam] = teamId;
                        }

                        if (teamId is null)
                        {
                            _logger.LogWarning("Team ExternalId={ExternalTeamId} ({Team}) not found for standing in league ExternalId={ExternalLeagueId} - skipping row.", dto.IdTeam, dto.StrTeam, league.ExternalId);
                            continue;
                        }

                        // NAMERNO NE po ExternalId (dto.IdStanding) - potvrdjeno u praksi da
                        // TheSportsDB dodeljuje NOV idStanding SVAKI PUT kad se tabela ponovo
                        // izracuna (npr. posle svakog kola), iako predstavlja ISTI konceptualni
                        // red (ovaj tim, ova liga, ova sezona). Upsert po ExternalId je zato
                        // stalno pravio NOVE redove umesto da azurira postojeci - potvrdjeno
                        // dupliranje do 60 redova za isti tim u nekim ligama. Logicki kljuc
                        // (LeagueId+TeamId+Season) je ono sto STVARNO identifikuje jedan red.
                        var standing = await _context.Standings.FirstOrDefaultAsync(
                            x => x.LeagueId == league.Id && x.TeamId == teamId.Value && x.Season == (dto.StrSeason ?? season), ct);

                        if (standing is null)
                        {
                            standing = new Standings();
                            _context.Standings.Add(standing);
                        }

                        // ExternalId i dalje cuvamo (SyncedEntity zahteva ga, i koristan je za
                        // debug) - samo vise NIJE kljuc po kome trazimo postojeci red.
                        standing.ExternalId = dto.IdStanding;
                        standing.Rank = int.TryParse(dto.IntRank, out var rank) ? rank : 0;
                        standing.TeamId = teamId.Value;
                        standing.LeagueId = league.Id;
                        standing.Season = dto.StrSeason ?? season;
                        standing.Group = dto.StrGroup;
                        standing.Form = dto.StrForm;
                        standing.Description = dto.StrDescription;
                        standing.MatchesPlayed = int.TryParse(dto.IntPlayed, out var played) ? played : 0;
                        standing.MatchesWon = int.TryParse(dto.IntWin, out var won) ? won : 0;
                        standing.MatchesLost = int.TryParse(dto.IntLoss, out var lost) ? lost : 0;
                        standing.MatchesDraw = int.TryParse(dto.IntDraw, out var draw) ? draw : 0;
                        standing.GoalsFor = int.TryParse(dto.IntGoalsFor, out var goalsFor) ? goalsFor : 0;
                        standing.GoalsAgainst = int.TryParse(dto.IntGoalsAgainst, out var goalsAgainst) ? goalsAgainst : 0;
                        standing.GoalDifference = int.TryParse(dto.IntGoalDifference, out var goalDifference) ? goalDifference : 0;
                        standing.Points = int.TryParse(dto.IntPoints, out var points) ? points : 0;
                        standing.LastSyncedAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync(ct);
                }

                /// <summary>
                /// RUCNI posao - tabela za SVAKU ligu u bazi. Automatsko osvezavanje ide
                /// po-zavrsenom-mecu (SyncLiveMatchStatisticsAsync) - ovo je za "osvezi sve
                /// odjednom" (npr. posle prvog uvodjenja Standings tabele).
                /// </summary>
                public async Task SyncStandingsAsync(CancellationToken ct = default)
                {
                    var leagues = await _context.Leagues.ToListAsync(ct);

                    foreach (var league in leagues)
                    {
                        try
                        {
                            await SyncStandingsForLeagueAsync(league, ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "SyncStandingsForLeagueAsync nije uspeo za ligu ExternalId={ExternalLeagueId} ({League}).", league.ExternalId, league.Name);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to sync standings for League (ExternalId={league.ExternalId}, Name={league.Name}).", ex, ct);
                        }
                    }
                }

                /// <summary>
                /// SKUP, JEDNOKRATAN posao - za svaku League u bazi, ide unazad kroz sezone
                /// (generisane iz League.CurrentSeason, isti helper kao SyncHistoricalMatchesAsync)
                /// i povlaci tabelu (Standings) svake sezone dok SportsDB ima podataka. NE ide u
                /// redovan ciklus - samo rucno, preko admin endpointa.
                /// </summary>
                public async Task SyncHistoricalStandingsAsync(CancellationToken ct = default)
                {
                    var leagues = await _context.Leagues.ToListAsync(ct);

                    foreach (var league in leagues)
                    {
                        try
                        {
                            if (_context.Entry(league).State == EntityState.Detached)
                            {
                                _context.Entry(league).State = EntityState.Unchanged;
                            }

                            if (string.IsNullOrWhiteSpace(league.CurrentSeason))
                            {
                                _logger.LogInformation("League ExternalId={ExternalLeagueId} ({League}) has no CurrentSeason set - cannot generate historical seasons, skipping.", league.ExternalId, league.Name);
                                continue;
                            }

                            const int maxConsecutiveMisses = 3;
                            var consecutiveMisses = 0;
                            var seasonsFetched = 0;

                            foreach (var season in GenerateSeasonsGoingBack(league.CurrentSeason))
                            {
                                List<Infrastructure.Integrations.DTO.SportsDbStanding> seasonStandings;

                                try
                                {
                                    seasonStandings = await _client.GetLeagueStandingsAsync(league.ExternalId, season, ct);
                                }
                                catch (SportsDbApiException apiEx) when (apiEx.Message.Contains("No data found"))
                                {
                                    seasonStandings = new List<Infrastructure.Integrations.DTO.SportsDbStanding>();
                                }

                                // Prazna lista - i eksplicitna "No data found" greska I "200 OK sa
                                // praznim nizom" (SportsDB ume oboje) - tretiraju se isto, kao promasaj.
                                if (seasonStandings.Count == 0)
                                {
                                    consecutiveMisses++;

                                    if (consecutiveMisses >= maxConsecutiveMisses)
                                    {
                                        _logger.LogInformation(
                                            "League ExternalId={ExternalLeagueId} ({League}): {Misses} consecutive seasons with no standings data - stopping historical backfill at season {Season}.",
                                            league.ExternalId, league.Name, consecutiveMisses, season);
                                        break;
                                    }

                                    continue;
                                }

                                consecutiveMisses = 0;
                                seasonsFetched++;

                                await UpsertStandingsAsync(league, seasonStandings, season, ct);
                            }

                            _logger.LogInformation("Historical standings backfill done for league ExternalId={ExternalLeagueId} ({League}) - {Seasons} seasons found.", league.ExternalId, league.Name, seasonsFetched);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Historical standings sync nije uspeo za ligu ExternalId={ExternalLeagueId}.", league.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed historical standings sync for League (ExternalId={league.ExternalId}, Name={league.Name}).", ex, ct);
                        }
                    }
                }
            #endregion


            #region Upsert upcoming lineups
            /// Postava POCEV OD 2 dana pre meca, na svakih ~2h - sastav cesto nije poznat sve
            /// do par sati pred sam mec, pa proveravamo periodicno unapred. Mecevi koji su vec
            /// u live prozoru se ovde takodje mogu naci (preklapanje je bezopasno - isti upsert),
            /// ali live tracker (na 2 min) ce ionako biti cesci za njih.

            public async Task SyncUpcomingLineupsAsync(CancellationToken ct = default)
                {
                    var now = DateTime.UtcNow;
                    var preMatchWindowEnd = now.AddDays(2);

                    var upcomingMatches = await _context.Matches
                        .Where(m => m.EventDate > now && m.EventDate <= preMatchWindowEnd)
                        .Where(m => !FinishedMatchStatuses.Contains(m.Status))
                        .ToListAsync(ct);

                    _logger.LogInformation("SyncUpcomingLineupsAsync: {Count} meceva u sledeca 2 dana za proveru postave.", upcomingMatches.Count);

                    foreach (var match in upcomingMatches)
                    {
                        try
                        {
                            if (_context.Entry(match).State == EntityState.Detached)
                            {
                                _context.Entry(match).State = EntityState.Unchanged;
                            }

                            await UpsertLineupsForMatchAsync(match, ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Sync unapred-postave nije uspeo za mec ExternalId={ExternalEventId}.", match.ExternalId);
                            _context.ChangeTracker.Clear();
                            await LogErrorAsync($"Failed to sync upcoming lineup for Match (ExternalId={match.ExternalId}, Name={match.Name}).", ex, ct);
                        }
                    }
            }
        #endregion

            #region Upsert lineups
        /// Deljena upsert logika za postavu - koriste je SyncMatchDetailsAsync (posle meca),
        /// live tracker (tokom meca) i SyncUpcomingLineupsAsync (pre meca). Upsert po
        /// (EventId, PlayerId) - NE po ExternalId (idLineup se menja izmedju poziva, isti
        /// razlog kao kod EventStatistic).
        private async Task UpsertLineupsForMatchAsync(Match match, CancellationToken ct)
        {
            var lineupDtos = await FetchListSafeAsync(
                () => _client.GetEventLineupAsync(match.ExternalId, ct), $"lineup for match ExternalId={match.ExternalId} ({match.Name})", ct);

            foreach (var lineupDto in lineupDtos)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.ExternalId == lineupDto.IdPlayer, ct);

                if (player is null)
                {
                    var detail = await FetchSingleSafeAsync(
                        () => _client.GetPlayerDetailAsync(lineupDto.IdPlayer, ct), $"player detail ExternalId={lineupDto.IdPlayer} ({lineupDto.StrPlayer}) via lineup", ct);

                    if (detail is not null)
                    {
                        var (firstName, lastName) = SplitPlayerName(detail.StrPlayer, detail.StrLastName);

                        player = new Player
                        {
                            ExternalId = lineupDto.IdPlayer,
                            FirstName = firstName,
                            LastName = lastName,
                            ImageUrl = detail.StrThumb ?? string.Empty,
                            BirthLocation = detail.StrBirthLocation ?? string.Empty,
                            Position = detail.StrPosition ?? lineupDto.StrPosition ?? string.Empty,
                            Description = detail.StrDescriptionEN,
                            Status = detail.StrStatus ?? string.Empty,
                            Weight = detail.StrWeight ?? string.Empty,
                            Height = detail.StrHeight ?? string.Empty,
                            Worth = detail.StrWage,
                            FacebookLink = detail.StrFacebook,
                            InstagramLink = detail.StrInstagram,
                            TwitterLink = detail.StrTwitter,
                            YouTubeLink = detail.StrYoutube
                        };

                        if (DateOnly.TryParse(detail.DateBorn, out var birthDate))
                        {
                            player.BirthDate = birthDate;
                        }

                        if (DateOnly.TryParse(detail.DateDied, out var dateDied))
                        {
                            player.DateDied = dateDied;
                        }

                        _context.Players.Add(player);
                        await _context.SaveChangesAsync(ct);
                    }
                    else if (!string.IsNullOrWhiteSpace(lineupDto.StrPlayer))
                    {
                        player = new Player
                        {
                            ExternalId = lineupDto.IdPlayer,
                            FirstName = lineupDto.StrPlayer,
                            LastName = string.Empty,
                            ImageUrl = string.Empty,
                            BirthLocation = string.Empty,
                            Position = lineupDto.StrPosition ?? string.Empty,
                            Status = string.Empty,
                            Weight = string.Empty,
                            Height = string.Empty
                        };
                        _context.Players.Add(player);
                        await _context.SaveChangesAsync(ct);

                        _logger.LogInformation("lookup/player returned nothing for ExternalId={IdPlayer} - created minimal Player record from lineup name instead.", lineupDto.IdPlayer);
                    }
                    else
                    {
                        _logger.LogWarning("Player ExternalId={IdPlayer} not found and no strPlayer name available - skipping lineup entry for match ExternalId={ExternalEventId}.", lineupDto.IdPlayer, match.ExternalId);
                        continue;
                    }
                }

                var lineupTeam = await _context.Teams.FirstOrDefaultAsync(x => x.ExternalId == lineupDto.IdTeam, ct);

                if (lineupTeam is null)
                {
                    _logger.LogWarning("Team ExternalId={IdTeam} not found for lineup entry (player {IdPlayer}) in match ExternalId={ExternalEventId} - skipping this entry.", lineupDto.IdTeam, lineupDto.IdPlayer, match.ExternalId);
                    continue;
                }

                var lineup = await _context.EventLineups.FirstOrDefaultAsync(x => x.EventId == match.Id && x.PlayerId == player.Id, ct);

                if (lineup is null)
                {
                    lineup = new EventLineup { ExternalId = lineupDto.IdLineup, EventId = match.Id, PlayerId = player.Id };
                    _context.EventLineups.Add(lineup);
                }

                lineup.TeamId = lineupTeam.Id;
                lineup.Position = lineupDto.StrPosition ?? string.Empty;
                lineup.PositionShort = lineupDto.StrPositionShort ?? string.Empty;
                lineup.Formation = lineupDto.StrFormation;
                lineup.HomeGame = string.Equals(lineupDto.StrHome, "Yes", StringComparison.OrdinalIgnoreCase);
                lineup.IsSubstitute = string.Equals(lineupDto.StrSubstitute, "Yes", StringComparison.OrdinalIgnoreCase);
                lineup.Season = lineupDto.StrSeason;
                lineup.LastSyncedAt = DateTime.UtcNow;

                if (int.TryParse(lineupDto.IntSquadNumber, out var squadNumber))
                {
                    lineup.SquadNumber = squadNumber;
                }
            }

            await _context.SaveChangesAsync(ct);
        }
        #endregion

            #region Helper methods
        private static DateTime? ParseEventDate(Infrastructure.Integrations.DTO.SportsDbMatch dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.StrTimestamp) && DateTime.TryParse(dto.StrTimestamp, out var timestamp))
            {
                return DateTime.SpecifyKind(timestamp, DateTimeKind.Utc);
            }

            if (!string.IsNullOrWhiteSpace(dto.DateEvent))
            {
                var timePart = string.IsNullOrWhiteSpace(dto.StrTime) ? "00:00:00" : dto.StrTime;
                if (DateTime.TryParse($"{dto.DateEvent} {timePart}", out var combined))
                {
                    return DateTime.SpecifyKind(combined, DateTimeKind.Utc);
                }
            }

            return null;
        }


        private static (string FirstName, string LastName) SplitPlayerName(string fullName, string? lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                return (fullName, string.Empty);


            if (fullName.EndsWith(lastName, StringComparison.OrdinalIgnoreCase))
            {
                var firstName = fullName[..^lastName.Length].Trim();
                return (firstName, lastName);
            }

            return (fullName, lastName);
        }

        private static IEnumerable<string> GenerateSeasonsGoingBack(string currentSeason, int maxSeasons = 150)
        {
            var parts = currentSeason.Split('-');

            if (parts.Length == 2 && int.TryParse(parts[0], out var startYear) && int.TryParse(parts[1], out var endYear))
            {
                for (var i = 0; i < maxSeasons; i++)
                {
                    yield return $"{startYear - i}-{endYear - i}";
                }

                yield break;
            }

            if (int.TryParse(currentSeason, out var year))
            {
                for (var i = 0; i < maxSeasons; i++)
                {
                    yield return (year - i).ToString();
                }

                yield break;
            }


            yield return currentSeason;
        }

        private async Task<int> FixMissingFinishedStatusesAsync(CancellationToken ct)
        {
            var cutoff = DateTime.UtcNow - LiveMatchWindow;

            var affected = await _context.Matches
                .Where(m => m.Status == null || m.Status == string.Empty)
                .Where(m => m.HomeTeamScore != null && m.AwayTeamScore != null)
                .Where(m => m.EventDate < cutoff)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.Status, "FT"), ct);

            if (affected > 0)
            {
                _logger.LogInformation("FixMissingFinishedStatusesAsync: {Count} meceva sa rezultatom a bez statusa - postavljeno na FT.", affected);
            }

            return affected;
        }
        #endregion
        
        public async Task SyncAllAsync(CancellationToken ct = default)
        {
            await SyncSportsAsync(ct);
            await SyncLeagueAndTeams(ct);
            await SyncVenuesAsync(ct);
        }

            #region Sync event TV broadcasts
            // Raspored osvezavanja zavisi od toga koliko je mec blizu (TV lista se objavljuje kasno i
            // menja se cesce sto je mec blizi). Mecevi dalji od TvHorizon se ne osvezavaju uopste -
            // njih pokriva hook pri prvom upisu meca (SyncMatchesAsync), a ulaze u prozor kad se priblize.
            private const int TvMaxMatchesPerRun = 500;
            private static readonly TimeSpan TvHorizon = TimeSpan.FromDays(15);
            private static readonly TimeSpan TvNearWindow = TimeSpan.FromHours(48);
            private static readonly TimeSpan TvMidWindow = TimeSpan.FromDays(7);
            private static readonly TimeSpan TvNearRefresh = TimeSpan.FromHours(6);
            private static readonly TimeSpan TvMidRefresh = TimeSpan.FromHours(12);
            private static readonly TimeSpan TvFarRefresh = TimeSpan.FromHours(48);

            private sealed record TvMatchRef(long Id, string ExternalId, string Name, DateTime EventDate);

            /// <summary>
            /// RUCNO okidanje - prolazi kroz mecove iz Match tabele (vidi EventTvSyncMode: svi / samo
            /// neobradjeni / samo oni sa pogresnim BroadcastTime). Ponovno pokretanje je BEZBEDNO -
            /// upsert po (EventId, ChannelId) + unique indeksi, bez duplikata.
            /// </summary>
            public async Task SyncEventTvAsync(EventTvSyncMode mode = EventTvSyncMode.All, CancellationToken ct = default)
            {
                IQueryable<Match> query = _context.Matches.AsNoTracking();

                query = mode switch
                {
                    EventTvSyncMode.Unsynced => query.Where(m => m.TvSyncedAt == null),

                    // Redovi koje je napisala STARA verzija (BroadcastTime = trenutak sync-a umesto
                    // pravog vremena emitovanja) - prepoznaju se po tome sto je BroadcastTime u
                    // +-10 min od LastSyncedAt istog reda.
                    EventTvSyncMode.Repair => query.Where(m => m.TvBroadcasts.Any(t =>
                        t.LastSyncedAt != null
                        && t.BroadcastTime >= t.LastSyncedAt.Value.AddMinutes(-10)
                        && t.BroadcastTime <= t.LastSyncedAt.Value.AddMinutes(10))),

                    _ => query
                };

                var matches = await query
                    .Select(m => new TvMatchRef(m.Id, m.ExternalId, m.Name, m.EventDate))
                    .ToListAsync(ct);

                _logger.LogInformation("SyncEventTvAsync (mode={Mode}): {Count} meceva za obradu.", mode, matches.Count);

                await SyncTvForMatchesAsync(matches, ct);
            }

            /// <summary>
            /// AUTOMATSKI (EventTvBackgroundService) - samo buduci mecevi u prozoru [sada, sada + TvHorizon]
            /// koji su "na redu": nikad proveravani, ili su stariji od intervala za svoju blizinu
            /// (do 2 dana: 6h, do 7 dana: 12h, dalje: 48h). Najstarije provereni prvi, najvise
            /// TvMaxMatchesPerRun po ciklusu - pa posle pauze/gasenja nista nije "propusteno",
            /// samo se sustize u narednim ciklusima.
            /// </summary>
            public async Task SyncUpcomingEventTvAsync(CancellationToken ct = default)
            {
                var now = DateTime.UtcNow;
                var nearEnd = now + TvNearWindow;
                var midEnd = now + TvMidWindow;
                var horizonEnd = now + TvHorizon;
                var nearStale = now - TvNearRefresh;
                var midStale = now - TvMidRefresh;
                var farStale = now - TvFarRefresh;

                var due = await _context.Matches
                    .AsNoTracking()
                    .Where(m => m.EventDate > now && m.EventDate <= horizonEnd)
                    .Where(m => m.TvSyncedAt == null
                        || (m.EventDate <= nearEnd && m.TvSyncedAt <= nearStale)
                        || (m.EventDate > nearEnd && m.EventDate <= midEnd && m.TvSyncedAt <= midStale)
                        || (m.EventDate > midEnd && m.TvSyncedAt <= farStale))
                    .OrderBy(m => m.TvSyncedAt)
                    .ThenBy(m => m.EventDate)
                    .Take(TvMaxMatchesPerRun)
                    .Select(m => new TvMatchRef(m.Id, m.ExternalId, m.Name, m.EventDate))
                    .ToListAsync(ct);

                if (due.Count == 0)
                    return;

                _logger.LogInformation("SyncUpcomingEventTvAsync: {Count} meceva na redu za osvezavanje TV rasporeda.", due.Count);

                await SyncTvForMatchesAsync(due, ct);
            }

            private async Task SyncTvForMatchesAsync(List<TvMatchRef> matches, CancellationToken ct)
            {
                var channelIdCache = new Dictionary<string, long>();

                foreach (var match in matches)
                {
                    try
                    {
                        await SyncTvForMatchAsync(match.Id, match.ExternalId, match.Name, match.EventDate, channelIdCache, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "TV sync nije uspeo za mec ExternalId={ExternalEventId}.", match.ExternalId);
                        _context.ChangeTracker.Clear();
                        await LogErrorAsync($"Failed to sync TV broadcasts for Match (ExternalId={match.ExternalId}, Name={match.Name}).", ex, ct);
                    }
                }
            }

            /// TV prenosi JEDNOG meca. Channel se upserta po ExternalId (idChannel) - kesiran u
            /// channelIdCache da isti kanal ne bude ponovo citan za svaki mec koji prenosi.
            /// Sam poziva SaveChangesAsync; izuzeci se propagiraju pozivaocu (koji ih hvata po mecu).
            /// Tek kad sve prodje bez izuzetka, upisuje se Match.TvSyncedAt.
            private async Task SyncTvForMatchAsync(long matchId, string matchExternalId, string matchName, DateTime matchEventDate,
                Dictionary<string, long> channelIdCache, CancellationToken ct)
            {
                var tvDtos = await FetchListSafeAsync(
                    () => _client.GetEventTvAsync(matchExternalId, ct), $"TV broadcasts for match ExternalId={matchExternalId} ({matchName})", ct);

                // Isti kanal moze da emituje isti mec vise puta (uzivo + repriza) - unique indeks
                // (EventId, ChannelId) dozvoljava JEDAN red, a dva "Add" u istom batch-u bi pukla.
                // Zadrzavamo najranije emitovanje (ISO format se sortira leksikografski).
                var uniqueByChannel = tvDtos
                    .Where(t => !string.IsNullOrWhiteSpace(t.IdChannel) && !string.IsNullOrWhiteSpace(t.StrChannel))
                    .GroupBy(t => t.IdChannel)
                    .Select(g => g.OrderBy(t => t.StrTimeStamp).First());

                var currentChannelIds = new List<long>();

                foreach (var tvDto in uniqueByChannel)
                {
                    if (!channelIdCache.TryGetValue(tvDto.IdChannel, out var channelId))
                    {
                        channelId = await ResolveChannelIdAsync(tvDto, ct);
                        channelIdCache[tvDto.IdChannel] = channelId;
                    }

                    currentChannelIds.Add(channelId);

                    // BroadcastTime = KAD kanal emituje (UTC, isto kao EventDate) - moze se razlikovati od pocetka
                    // meca (odlozeni prenos/repriza). Ako provajder ne posalje vreme, uzimamo pocetak meca.
                    var broadcastTime = DateTime.TryParse(tvDto.StrTimeStamp, out var parsedTime)
                        ? DateTime.SpecifyKind(parsedTime, DateTimeKind.Utc)
                        : matchEventDate;

                    var externalId = string.IsNullOrWhiteSpace(tvDto.Id) ? $"{tvDto.IdEvent}-{tvDto.IdChannel}" : tvDto.Id;

                    var broadcast = await _context.EventTvs.FirstOrDefaultAsync(
                        x => x.EventId == matchId && x.ChannelId == channelId, ct);

                    if (broadcast is null)
                    {
                        _context.EventTvs.Add(new EventTv
                        {
                            ExternalId = externalId,
                            EventId = matchId,
                            ChannelId = channelId,
                            BroadcastTime = broadcastTime,
                            LastSyncedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        broadcast.ExternalId = externalId;
                        broadcast.BroadcastTime = broadcastTime;
                        broadcast.LastSyncedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync(ct);

                // Kanal koji vise nije na listi provajdera (npr. promena prenosioca) mora da nestane i
                // kod nas - upsert sam samo dodaje. Brise se SAMO kad je odgovor NEPRAZAN: prazan odgovor
                // je dvosmislen ("nema prenosa" vs. privremeni problem provajdera) pa se ne sme koristiti
                // za brisanje dobrih podataka.
                if (currentChannelIds.Count > 0)
                {
                    await _context.EventTvs
                        .Where(x => x.EventId == matchId && !currentChannelIds.Contains(x.ChannelId))
                        .ExecuteDeleteAsync(ct);
                }

                var syncedAt = DateTime.UtcNow;
                await _context.Matches
                    .Where(m => m.Id == matchId)
                    .ExecuteUpdateAsync(s => s.SetProperty(m => m.TvSyncedAt, syncedAt), ct);
            }

            private async Task<long> ResolveChannelIdAsync(SportsDbEventTv tvDto, CancellationToken ct)
            {
                var channel = await _context.Channels.FirstOrDefaultAsync(c => c.ExternalId == tvDto.IdChannel, ct);

                if (channel is null)
                {
                    channel = new Channel
                    {
                        ExternalId = tvDto.IdChannel,
                        Name = tvDto.StrChannel,
                        Country = tvDto.StrCountry,
                        LogoUrl = tvDto.StrLogo,
                        LastSyncedAt = DateTime.UtcNow
                    };
                    _context.Channels.Add(channel);
                    await _context.SaveChangesAsync(ct); // treba nam channel.Id pre upisa EventTv FK-a
                }
                else
                {
                    channel.Name = tvDto.StrChannel;
                    channel.Country = tvDto.StrCountry;
                    channel.LogoUrl = tvDto.StrLogo;
                    channel.LastSyncedAt = DateTime.UtcNow;
                }

                return channel.Id;
            }
            #endregion

            #region Sync event timeline
            // Tok meca (golovi, kartoni, izmene) - TheSportsDB ga daje tek posle kraja meca, i potvrdjeno
            // samo za fudbal (po Postman testu), zato je spisak sportova konstanta koja se lako prosiruje.
            private static readonly string[] TimelineSports = { "Soccer" };
            private const int TimelineMaxMatchesPerRun = 300;
            private static readonly TimeSpan TimelineWindow = TimeSpan.FromDays(3);

            private sealed record TimelineMatchRef(long Id, string ExternalId, string Name);

            /// <summary>
            /// RUCNO okidanje - SVI zavrseni mecevi (FT/AET/PEN) iz baze (podrazumevano samo sportovi iz
            /// TimelineSports; allSports=true probava sve sportove, npr. da se proveri da li ih provajder
            /// uopste podrzava). Ponovno pokretanje je BEZBEDNO (upsert po idTimeline, bez duplikata).
            /// </summary>
            public async Task SyncEventTimelineAsync(TimelineSyncMode mode = TimelineSyncMode.All, bool allSports = false, CancellationToken ct = default)
            {
                // Mecevi sa rezultatom a praznim statusom moraju prvo da dobiju "zavrsen" status, inace
                // ostaju nevidljivi za upit ispod.
                await FixMissingFinishedStatusesAsync(ct);

                var query = _context.Matches
                    .AsNoTracking()
                    .Where(m => FinishedMatchStatuses.Contains(m.Status));

                if (!allSports)
                    query = query.Where(m => TimelineSports.Contains(m.League.Sport.Name));

                if (mode == TimelineSyncMode.Unsynced)
                    query = query.Where(m => m.TimelineSyncedAt == null);

                var matches = await query
                    .OrderBy(m => m.Id)
                    .Select(m => new TimelineMatchRef(m.Id, m.ExternalId, m.Name))
                    .ToListAsync(ct);

                _logger.LogInformation("SyncEventTimelineAsync (mode={Mode}, allSports={AllSports}): {Count} meceva za obradu.", mode, allSports, matches.Count);

                await SyncTimelineForMatchesAsync(matches, ct);
            }

            /// <summary>
            /// AUTOMATSKI (EventTimelineBackgroundService) - zavrseni mecevi iz poslednja 3 dana koji su
            /// "na redu": nikad proveravani, ili stariji od intervala za svoju starost (do 8h: 1h, do 24h: 6h,
            /// do 3 dana: 24h). Interval se ne prekida ni kad tok vec postoji - provajder moze da ispravi
            /// dogadjaje (npr. poponisten gol), a to zelimo da uhvatimo. Najstarije provereni prvi, najvise
            /// TimelineMaxMatchesPerRun po ciklusu.
            /// </summary>
            public async Task SyncRecentEventTimelineAsync(CancellationToken ct = default)
            {
                var now = DateTime.UtcNow;
                var windowStart = now - TimelineWindow;
                var youngEnd = now - TimeSpan.FromHours(8);
                var midEnd = now - TimeSpan.FromHours(24);
                var youngStale = now - TimeSpan.FromHours(1);
                var midStale = now - TimeSpan.FromHours(6);
                var oldStale = now - TimeSpan.FromHours(24);

                var due = await _context.Matches
                    .AsNoTracking()
                    .Where(m => FinishedMatchStatuses.Contains(m.Status))
                    .Where(m => m.EventDate >= windowStart && m.EventDate <= now)
                    .Where(m => TimelineSports.Contains(m.League.Sport.Name))
                    .Where(m => m.TimelineSyncedAt == null
                        || (m.EventDate > youngEnd && m.TimelineSyncedAt <= youngStale)
                        || (m.EventDate <= youngEnd && m.EventDate > midEnd && m.TimelineSyncedAt <= midStale)
                        || (m.EventDate <= midEnd && m.TimelineSyncedAt <= oldStale))
                    .OrderBy(m => m.TimelineSyncedAt)
                    .ThenBy(m => m.EventDate)
                    .Take(TimelineMaxMatchesPerRun)
                    .Select(m => new TimelineMatchRef(m.Id, m.ExternalId, m.Name))
                    .ToListAsync(ct);

                if (due.Count == 0)
                    return;

                _logger.LogInformation("SyncRecentEventTimelineAsync: {Count} meceva na redu za osvezavanje toka.", due.Count);

                await SyncTimelineForMatchesAsync(due, ct);
            }

            private async Task SyncTimelineForMatchesAsync(List<TimelineMatchRef> matches, CancellationToken ct)
            {
                var teamIdCache = new Dictionary<string, long>();
                var playerIdCache = new Dictionary<string, long>();

                foreach (var match in matches)
                {
                    try
                    {
                        await SyncTimelineForMatchAsync(match, teamIdCache, playerIdCache, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Timeline sync nije uspeo za mec ExternalId={ExternalEventId}.", match.ExternalId);
                        _context.ChangeTracker.Clear();
                        await LogErrorAsync($"Failed to sync timeline for Match (ExternalId={match.ExternalId}, Name={match.Name}).", ex, ct);
                    }
                }
            }

            /// Tok JEDNOG meca. Upsert po idTimeline (ExternalId); redovi kojih nema u NEPRAZNOM odgovoru se
            /// brisu (ispravke provajdera), a prazan odgovor nikad ne brise dobre podatke (dvosmislen je:
            /// "nema toka" vs. "jos nije objavljen"). Match.TimelineSyncedAt se upisuje tek kad sve prodje.
            private async Task SyncTimelineForMatchAsync(TimelineMatchRef match, Dictionary<string, long> teamIdCache,
                Dictionary<string, long> playerIdCache, CancellationToken ct)
            {
                var dtos = await FetchListSafeAsync(
                    () => _client.GetEventTimelineAsync(match.ExternalId, ct), $"timeline for match ExternalId={match.ExternalId} ({match.Name})", ct);

                var currentExternalIds = new List<string>();

                foreach (var dto in dtos)
                {
                    if (string.IsNullOrWhiteSpace(dto.IdTimeline) || string.IsNullOrWhiteSpace(dto.StrTimeline))
                        continue;

                    var teamId = await ResolveTeamIdByExternalIdAsync(dto.IdTeam, teamIdCache, ct);
                    if (teamId is null)
                    {
                        _logger.LogWarning("Team ExternalId={IdTeam} not found for timeline entry ExternalId={IdTimeline} (match ExternalId={ExternalEventId}) - skipping.", dto.IdTeam, dto.IdTimeline, match.ExternalId);
                        continue;
                    }

                    var playerId = await ResolvePlayerIdForTimelineAsync(dto.IdPlayer, dto.StrPlayer, dto.StrCutout, playerIdCache, ct);
                    var assistantId = await ResolvePlayerIdForTimelineAsync(dto.IdAssist, dto.StrAssist, null, playerIdCache, ct);

                    var entry = await _context.EventTimelines.FirstOrDefaultAsync(x => x.ExternalId == dto.IdTimeline, ct);
                    if (entry is null)
                    {
                        entry = new EventTimeline { ExternalId = dto.IdTimeline };
                        _context.EventTimelines.Add(entry);
                    }

                    entry.EventId = match.Id;
                    entry.Type = dto.StrTimeline;
                    entry.Detail = NullIfBlankOrNullLiteral(dto.StrTimelineDetail);
                    entry.Home = string.Equals(dto.StrHome, "Yes", StringComparison.OrdinalIgnoreCase);
                    entry.APIId =NullIfBlankOrNullLiteral(dto.IdApiFootball);
                    entry.PlayerId = playerId;
                    entry.AssistantId = assistantId;
                    entry.TeamId = teamId.Value;
                    entry.Comment = NullIfBlankOrNullLiteral(dto.StrComment);
                    entry.Minute = int.TryParse(dto.IntTime, out var minute) ? minute : null;
                    entry.Period = NullIfBlankOrNullLiteral(dto.StrPeriod);
                    entry.LastSyncedAt = DateTime.UtcNow;

                    currentExternalIds.Add(dto.IdTimeline);
                }

                await _context.SaveChangesAsync(ct);

                if (currentExternalIds.Count > 0)
                {
                    await _context.EventTimelines
                        .Where(x => x.EventId == match.Id && !currentExternalIds.Contains(x.ExternalId))
                        .ExecuteDeleteAsync(ct);
                }

                var syncedAt = DateTime.UtcNow;
                await _context.Matches
                    .Where(m => m.Id == match.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(m => m.TimelineSyncedAt, syncedAt), ct);
            }

            private async Task<long?> ResolveTeamIdByExternalIdAsync(string? externalTeamId, Dictionary<string, long> cache, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(externalTeamId))
                    return null;

                if (cache.TryGetValue(externalTeamId, out var cached))
                    return cached;

                var id = await _context.Teams
                    .Where(t => t.ExternalId == externalTeamId)
                    .Select(t => (long?)t.Id)
                    .FirstOrDefaultAsync(ct);

                if (id is not null)
                    cache[externalTeamId] = id.Value;

                return id;
            }

            /// Igrac iz toka meca -> nas Player.Id (po ExternalId). "0"/prazno = nema igraca (npr. gol bez
            /// asistenta) -> null. Ako igraca nema u bazi, pravi se MINIMALNI stub (kao iz postave meca) - ime
            /// dolazi iz samog odgovora, bez dodatnog API poziva; SyncStubPlayerProfilesAsync ga kasnije dopuni.
            private async Task<long?> ResolvePlayerIdForTimelineAsync(string? externalPlayerId, string? playerName, string? imageUrl,
                Dictionary<string, long> cache, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(externalPlayerId) || externalPlayerId == "0")
                    return null;

                if (cache.TryGetValue(externalPlayerId, out var cached))
                    return cached;

                var id = await _context.Players
                    .Where(p => p.ExternalId == externalPlayerId)
                    .Select(p => (long?)p.Id)
                    .FirstOrDefaultAsync(ct);

                if (id is null)
                {
                    if (string.IsNullOrWhiteSpace(playerName))
                        return null;

                    var (firstName, lastName) = SplitPlayerName(playerName, null);
                    var stub = new Player
                    {
                        ExternalId = externalPlayerId,
                        FirstName = firstName,
                        LastName = lastName,
                        ImageUrl = imageUrl ?? string.Empty,
                        BirthLocation = string.Empty,
                        Position = string.Empty,
                        Status = string.Empty,
                        Weight = string.Empty,
                        Height = string.Empty
                    };
                    _context.Players.Add(stub);
                    await _context.SaveChangesAsync(ct);

                    id = stub.Id;
                    _logger.LogInformation("Player ExternalId={IdPlayer} ({Name}) not in database - created minimal stub Player from timeline entry.", externalPlayerId, playerName);
                }

                cache[externalPlayerId] = id.Value;
                return id;
            }

            // Provajder umesto null salje literalni tekst "NULL" (npr. strComment).
            private static string? NullIfBlankOrNullLiteral(string? value)
                => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase) ? null : value;
            #endregion

        /// Poziva dati fetch delegat i tretira "No data found" kao normalnu, ocekivanu
        /// situaciju (ne gresku) - vraca praznu listu umesto da baci dalje ka Logs tabeli.
        private async Task<List<T>> FetchListSafeAsync<T>(Func<Task<List<T>>> fetch, string context, CancellationToken ct)
        {
            try
            {
                return await fetch();
            }
            catch (SportsDbApiException apiEx) when (apiEx.Message.Contains("No data found"))
            {
                _logger.LogInformation("No data found for {Context} - skipping, this is expected sometimes.", context);
                return new List<T>();
            }
        }

        // "Ice Hockey" -> "ice_hockey", "Soccer" -> "soccer" - potvrdjeno u praksi (curl) za
        // sva 4 pracena sporta van fudbala, tacno ovaj oblik ocekuje /livescore/{sport}.
        private static string ToLiveScoreSportSegment(string sportName) => sportName.ToLowerInvariant().Replace(' ', '_');

        /// Isto kao FetchListSafeAsync, samo za pojedinacan (nullable) rezultat.
        private async Task<T?> FetchSingleSafeAsync<T>(Func<Task<T?>> fetch, string context, CancellationToken ct) where T : class
        {
            try
            {
                return await fetch();
            }
            catch (SportsDbApiException apiEx) when (apiEx.Message.Contains("No data found"))
            {
                _logger.LogInformation("No data found for {Context} - skipping, this is expected sometimes.", context);
                return null;
            }
        }

        /// odvojeno od SaveChangesAsync poziva u glavnom toku - da greška pri logovanju
        /// ne obori ceo sync
        private async Task LogErrorAsync(string context, Exception ex, CancellationToken ct)
        {
            try
            {
                var apiEx = ex as SportsDbApiException;

                _context.Logs.Add(new Log
                {
                    Exception = $"{context} {ex.Message}",
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace,
                    HttpMethod = apiEx?.HttpMethod,
                    HttpUrl = apiEx?.Url
                });
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception logEx)
            {
                _logger.LogCritical(logEx, "Failed to write error log to database. Original error: {Context}", context);
            }
        }
        #endregion
    }
}
