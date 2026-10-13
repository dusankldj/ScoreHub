using Infrastructure.Integrations;
using Infrastructure.Integrations.DTO;
using Infrastructure.Integrations.Exceptions;
using Infrastructure.Integrations.SportsDB;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.Json;

namespace Infrastructure
{
    /// <summary>
    /// V2 API. Autentifikacija ide preko "X-API-KEY" header-a.
    /// Rute su REST stil ("/lookup/league/{id}", "/list/teams/{id}", "/all/sports"), bez ".php" nastavka.
    /// </summary>
    public class SportsDbClient : ISportsDbClient
    {
        private const string BaseUrl = "https://www.thesportsdb.com/api/v2/json";

        // Standings (lookuptable.php) postoji SAMO u V1 API-ju - nema V2 ekvivalent.
        private const string V1BaseUrl = "https://www.thesportsdb.com/api/v1/json";
        private const string HttpMethodName = "GET";

        private readonly HttpClient _http;
        private readonly SportsOptions _options;

        public SportsDbClient(HttpClient http, IOptions<SportsOptions> options)
        {
            _http = http;
            _options = options.Value;

            if (!_http.DefaultRequestHeaders.Contains("X-API-KEY"))
            {
                _http.DefaultRequestHeaders.Add("X-API-KEY", _options.ApiKey);
            }
        }

        public async Task<SportsDbLeague?> GetLeagueAsync(string externalLeagueId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/league/{externalLeagueId}";
            var items = await GetFirstArrayAsync<SportsDbLeague>(url, ct);
            return items.FirstOrDefault();
        }

        public async Task<List<SportsDbTeam>> GetTeamsForLeagueAsync(string externalLeagueId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/list/teams/{externalLeagueId}";
            return await GetFirstArrayAsync<SportsDbTeam>(url, ct);
        }

        public async Task<List<SportsDbSport>> GetSportsAsync(CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/all/sports";
            return await GetFirstArrayAsync<SportsDbSport>(url, ct);
        }

        public async Task<SportsDbTeam?> GetTeamDetailAsync(string externalTeamId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/team/{externalTeamId}";
            var items = await GetFirstArrayAsync<SportsDbTeam>(url, ct);
            return items.FirstOrDefault();
        }

        public async Task<SportsDbVenue?> SearchVenueAsync(string venueName, CancellationToken ct = default)
        {
            var encodedName = Uri.EscapeDataString(venueName);
            var url = $"{BaseUrl}/search/venue/{encodedName}";
            var items = await GetFirstArrayAsync<SportsDbVenue>(url, ct);
            return items.FirstOrDefault();
        }

        public async Task<List<SportsDbPlayer>> GetPlayersForTeamAsync(string externalTeamId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/list/players/{externalTeamId}";
            return await GetFirstArrayAsync<SportsDbPlayer>(url, ct);
        }

        public async Task<SportsDbPlayer?> GetPlayerDetailAsync(string externalPlayerId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/player/{externalPlayerId}";
            var items = await GetFirstArrayAsync<SportsDbPlayer>(url, ct);
            return items.FirstOrDefault();
        }

        public async Task<List<SportsDbMatch>> GetNextMatchesForLeagueAsync(string externalLeagueId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/schedule/next/league/{externalLeagueId}";
            return await GetFirstArrayAsync<SportsDbMatch>(url, ct);
        }

        public async Task<List<SportsDbMatch>> GetPreviousMatchesForLeagueAsync(string externalLeagueId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/schedule/previous/league/{externalLeagueId}";
            return await GetFirstArrayAsync<SportsDbMatch>(url, ct);
        }

        public async Task<List<SportsDbMatch>> GetSeasonMatchesForLeagueAsync(string externalLeagueId, string season, CancellationToken ct = default)
        {
            var encodedSeason = Uri.EscapeDataString(season);
            var url = $"{BaseUrl}/schedule/league/{externalLeagueId}/{encodedSeason}";
            return await GetFirstArrayAsync<SportsDbMatch>(url, ct);
        }

        public async Task<SportsDbMatch?> GetEventDetailAsync(string externalEventId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/event/{externalEventId}";
            var items = await GetFirstArrayAsync<SportsDbMatch>(url, ct);
            return items.FirstOrDefault();
        }

        public async Task<List<SportsDbEventStatistic>> GetEventStatisticsAsync(string externalEventId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/event_stats/{externalEventId}";
            return await GetFirstArrayAsync<SportsDbEventStatistic>(url, ct);
        }

        public async Task<List<SportsDbEventLineup>> GetEventLineupAsync(string externalEventId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/event_lineup/{externalEventId}";
            return await GetFirstArrayAsync<SportsDbEventLineup>(url, ct);
        }

        public async Task<List<SportsDbTeamEquipment>> GetTeamEquipmentAsync(string externalTeamId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/team_equipment/{externalTeamId}";
            return await GetFirstArrayAsync<SportsDbTeamEquipment>(url, ct);
        }

        public async Task<List<SportsDbPlayerStatistic>> GetPlayerStatisticsAsync(string externalPlayerId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/player_stats/{externalPlayerId}";
            return await GetFirstArrayAsync<SportsDbPlayerStatistic>(url, ct);
        }

        public async Task<List<SportsDbPlayerFormerTeam>> GetPlayerFormerTeamsAsync(string externalPlayerId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/player_teams/{externalPlayerId}";
            return await GetFirstArrayAsync<SportsDbPlayerFormerTeam>(url, ct);
        }

        public async Task<List<SportsDbPlayerHonour>> GetPlayerHonoursAsync(string externalPlayerId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/player_honours/{externalPlayerId}";
            return await GetFirstArrayAsync<SportsDbPlayerHonour>(url, ct);
        }

        public async Task<List<SportsDbPlayerMilestone>> GetPlayerMilestonesAsync(string externalPlayerId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/player_milestones/{externalPlayerId}";
            return await GetFirstArrayAsync<SportsDbPlayerMilestone>(url, ct);
        }

        // V1 (samo lookuptable.php/standings) je ogranicen na 100 zahteva/min - potvrdjeno u
        // praksi (429 "Rate Limit exceeded"). V2 (sve ostalo u ovom klijentu) ima znatno veci
        // limit, pa se throttle NAMERNO primenjuje samo ovde, ne u deljenom GetFirstArrayAsync -
        // ne zelimo da usporimo V2 pozive koji ga ne trebaju. Staticko stanje (ne po-instanci)
        // jer se SportsDbClient pravi nanovo za svaki DI scope, a ogranicenje je GLOBALNO za
        // ceo proces (bulk istorijski sync + redovan 2-min live tracker mogu raditi paralelno).
        private static readonly SemaphoreSlim V1RateLimitGate = new(1, 1);
        private static DateTime _lastV1RequestUtc = DateTime.MinValue;
        private static readonly TimeSpan MinV1RequestInterval = TimeSpan.FromMilliseconds(700); // ~85/min, ispod stvarnog limita

        public async Task<List<SportsDbStanding>> GetLeagueStandingsAsync(string externalLeagueId, string? season = null, CancellationToken ct = default)
        {
            var url = $"{V1BaseUrl}/{_options.ApiKey}/lookuptable.php?l={externalLeagueId}";

            if (!string.IsNullOrWhiteSpace(season))
            {
                url += $"&s={Uri.EscapeDataString(season)}";
            }

            await ThrottleV1Async(ct);
            return await GetFirstArrayAsync<SportsDbStanding>(url, ct);
        }

        public async Task<List<SportsDbLiveScore>> GetLiveScoresAsync(string sport, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/livescore/{sport}";
            return await GetFirstArrayAsync<SportsDbLiveScore>(url, ct);
        }

        public async Task<List<SportsDbEventTv>> GetEventTvAsync(string externalEventId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/event_tv/{externalEventId}";
            return await GetFirstArrayAsync<SportsDbEventTv>(url, ct);
        }

        public async Task<List<SportsDbEventTimeline>> GetEventTimelineAsync(string externalEventId, CancellationToken ct = default)
        {
            var url = $"{BaseUrl}/lookup/event_timeline/{externalEventId}";
            return await GetFirstArrayAsync<SportsDbEventTimeline>(url, ct);
        }

        private static async Task ThrottleV1Async(CancellationToken ct)
        {
            await V1RateLimitGate.WaitAsync(ct);
            try
            {
                var wait = MinV1RequestInterval - (DateTime.UtcNow - _lastV1RequestUtc);

                if (wait > TimeSpan.Zero)
                {
                    await Task.Delay(wait, ct);
                }

                _lastV1RequestUtc = DateTime.UtcNow;
            }
            finally
            {
                V1RateLimitGate.Release();
            }
        }

        /// <summary>
        /// Uzima prvu (jedinu očekivanu) listu iz odgovora, bez obzira na tačno ime ključa.
        /// Prepoznaje i API-nivo greške (npr. {"Message": "..."}) PRE pokušaja deserijalizacije
        /// u očekivan oblik podataka, da bismo dobili jasnu, čitljivu poruku umesto
        /// zbunjujuće JSON deserialization greške.
        /// </summary>
        private async Task<List<T>> GetFirstArrayAsync<T>(string url, CancellationToken ct)
        {
            string raw;
            System.Net.HttpStatusCode statusCode;

            try
            {
                var response = await _http.GetAsync(url, ct);
                statusCode = response.StatusCode;
                raw = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    throw new SportsDbApiException(
                        $"SportsDB returned HTTP {(int)statusCode} ({statusCode}). Body: {Truncate(raw)}",
                        url, HttpMethodName);
                }
            }
            catch (SportsDbApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SportsDbApiException($"Network error while calling SportsDB: {ex.Message}", url, HttpMethodName, ex);
            }

            // API ponekad vrati 200 OK sa POTPUNO praznim telom (ne "{}"/"[]", nego bukvalno
            // nula bajtova) kad nema podataka - JsonDocument.Parse na tome baca sirov
            // JsonException koji NE prolazi kroz "No data found" prepoznavanje ispod. Tretiramo
            // ga isto kao svaki drugi "nema podataka" odgovor.
            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new SportsDbApiException(
                    "SportsDB API returned an empty response body (No data found).",
                    url, HttpMethodName);
            }

            using var doc = JsonDocument.Parse(raw);

            // API ponekad vrati 200 OK sa {"Message": "..."} umesto stvarnih podataka
            // (npr. kad liga/tim nemaju dostupne podatke, ili neki drugi API-nivo problem).
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("Message", out var messageProp))
            {
                throw new SportsDbApiException(
                    $"SportsDB API returned an error message instead of data: \"{messageProp.GetString()}\"",
                    url, HttpMethodName);
            }

            try
            {
                var parsed = JsonSerializer.Deserialize<Dictionary<string, List<T>?>>(raw);
                var firstList = parsed?.Values.FirstOrDefault(v => v is not null);
                return firstList ?? new List<T>();
            }
            catch (Exception ex)
            {
                throw new SportsDbApiException($"Could not parse SportsDB response as expected data shape: {ex.Message}", url, HttpMethodName, ex);
            }
        }

        private static string Truncate(string s) => s.Length <= 300 ? s : s[..300] + "...";
    }
}
