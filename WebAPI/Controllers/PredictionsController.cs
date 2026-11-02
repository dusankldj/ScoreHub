using DataAccess;
using Domain.Entities;
using Domain.Predictions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.DTO.Input;
using WebAPI.DTO.Output;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/predictions")]

    [Authorize]
    public class PredictionsController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public PredictionsController(ScoreHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult<List<MyPredictionDTO>>> GetMyPredictions(CancellationToken ct)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized();

            return Ok(await LoadPredictionsAsync(userId.Value, ct));
        }

        // Prognoze drugog korisnika - SAMO za prijatelje (inace 403). Isti oblik odgovora kao "me".
        [HttpGet("user/{targetUserId:long}")]
        public async Task<ActionResult<List<MyPredictionDTO>>> GetUserPredictions(long targetUserId, CancellationToken ct)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized();

            if (userId.Value != targetUserId && !await _context.AreFriendsAsync(userId.Value, targetUserId, ct))
                return Forbid();

            return Ok(await LoadPredictionsAsync(targetUserId, ct));
        }

        private async Task<List<MyPredictionDTO>> LoadPredictionsAsync(long userId, CancellationToken ct)
        {
            var nowUtc = DateTime.UtcNow;
            var rules = await _context.LoadScoringRulesAsync(ct);

            var predictions = await _context.Predictions
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.MatchId,
                    p.PredictedHomeScore,
                    p.PredictedAwayScore,
                    p.Match.EventDate,
                    p.Match.Status,
                    p.Match.HomeTeamScore,
                    p.Match.AwayTeamScore,
                    SportId = p.Match.League.SportId,
                    HomeTeam = p.Match.HomeTeam.Name,
                    AwayTeam = p.Match.AwayTeam.Name
                })
                .ToListAsync(ct);

            var result = predictions
                .Select(p =>
                {
                    var rule = rules.For(p.SportId);
                    var outcome = PredictionScoring.Evaluate(p.Status, p.HomeTeamScore, p.AwayTeamScore, p.PredictedHomeScore, p.PredictedAwayScore, rule);

                    return new
                    {
                        p.MatchId, p.PredictedHomeScore, p.PredictedAwayScore, p.EventDate, p.Status,
                        p.HomeTeamScore, p.AwayTeamScore, p.HomeTeam, p.AwayTeam,
                        Outcome = outcome,
                        Points = PredictionScoring.Points(outcome, rule)
                    };
                })
                .OrderBy(p => p.EventDate)
                .Select(p => new MyPredictionDTO
                {
                    EventId = p.MatchId,
                    EventDate = p.EventDate,
                    HomeTeam = p.HomeTeam,
                    AwayTeam = p.AwayTeam,
                    HomeTeamScore = p.HomeTeamScore,
                    AwayTeamScore = p.AwayTeamScore,
                    // Odlozen mec ceka novi termin -> u listi je "Upcoming" (a ne "Finished" kako ga vidi MatchStateCalculator).
                    Status = PredictionScoring.IsPostponed(p.Status) ? "Upcoming" : MatchStateCalculator.ComputeState(p.EventDate, p.Status, nowUtc),
                    PredictedHomeScore = p.PredictedHomeScore,
                    PredictedAwayScore = p.PredictedAwayScore,
                    Outcome = p.Outcome.ToString(),
                    Points = p.Points
                })
                .ToList();

            return result;
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<PredictionDTO>> GetMyPrediction(long matchId, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var row = await _context.Predictions
                .AsNoTracking()
                .Where(p => p.UserId == userId.Value && p.MatchId == matchId)
                .Select(p => new
                {
                    p.PredictedHomeScore,
                    p.PredictedAwayScore,
                    p.Match.Status,
                    p.Match.HomeTeamScore,
                    p.Match.AwayTeamScore,
                    SportId = p.Match.League.SportId
                })
                .FirstOrDefaultAsync(ct);

            if (row is null)
                return NotFound();

            var rule = (await _context.LoadScoringRulesAsync(ct)).For(row.SportId);
            var outcome = PredictionScoring.Evaluate(row.Status, row.HomeTeamScore, row.AwayTeamScore, row.PredictedHomeScore, row.PredictedAwayScore, rule);

            return Ok(new PredictionDTO
            {
                PredictedHomeScore = row.PredictedHomeScore,
                PredictedAwayScore = row.PredictedAwayScore,
                Outcome = outcome.ToString(),
                Points = PredictionScoring.Points(outcome, rule)
            });
        }


        [HttpPut("match/{matchId}")]
        public async Task<IActionResult> SavePrediction(long matchId, [FromBody] SavePredictionDTO request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var match = await _context.Matches
                .AsNoTracking()
                .Where(m => m.Id == matchId)
                .Select(m => new { m.EventDate, m.Status, m.League.SportId })
                .FirstOrDefaultAsync(ct);

            if (match is null)
                return NotFound();

            // Ogranicenje po sportu (SportScoringRules.MaxScore) - npr. odbojka: najvise 3 seta po timu.
            var maxScore = (await _context.LoadScoringRulesAsync(ct)).For(match.SportId).MaxScore;
            if (maxScore is int max && (request.PredictedHomeScore > max || request.PredictedAwayScore > max))
                return BadRequest(new ErrorResponseDTO { Errors = { $"Scores for this sport must be between 0 and {max}." } });

            if (PredictionScoring.IsVoid(match.Status))
                return BadRequest(new ErrorResponseDTO { Errors = { "Predictions are closed - the match was cancelled." } });

            if (MatchStateCalculator.ComputeState(match.EventDate, match.Status, DateTime.UtcNow) != "Upcoming")
                return BadRequest(new ErrorResponseDTO { Errors = { "Predictions are closed - the match has already started." } });

            var prediction = await _context.Predictions.FirstOrDefaultAsync(p => p.UserId == userId.Value && p.MatchId == matchId, ct);
            if (prediction is null)
            {
                _context.Predictions.Add(new Prediction
                {
                    UserId = userId.Value,
                    MatchId = matchId,
                    PredictedHomeScore = request.PredictedHomeScore,
                    PredictedAwayScore = request.PredictedAwayScore
                });
            }
            else
            {
                prediction.PredictedHomeScore = request.PredictedHomeScore;
                prediction.PredictedAwayScore = request.PredictedAwayScore;
            }

            await _context.SaveChangesAsync(ct);
            return Ok();
        }

        // Statistika ulogovanog korisnika. Sve se racuna pri citanju iz rezultata meceva (PredictionScoring) -
        // ne iz kolona Prediction.Hit/Points, koje nisu pouzdane za mecove zavrsene dok servis nije radio.
        [HttpGet("me/stats")]
        public async Task<ActionResult<PredictionStatsDTO>> GetMyStats(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var mine = await _context.Predictions
                .AsNoTracking()
                .Where(p => p.UserId == userId.Value)
                .Select(p => new
                {
                    p.Match.EventDate,
                    p.Match.Status,
                    p.Match.HomeTeamScore,
                    p.Match.AwayTeamScore,
                    SportId = p.Match.League.SportId,
                    p.PredictedHomeScore,
                    p.PredictedAwayScore
                })
                .ToListAsync(ct);

            var rules = await _context.LoadScoringRulesAsync(ct);

            var evaluated = mine
                .Select(p =>
                {
                    var rule = rules.For(p.SportId);
                    var outcome = PredictionScoring.Evaluate(p.Status, p.HomeTeamScore, p.AwayTeamScore, p.PredictedHomeScore, p.PredictedAwayScore, rule);
                    return (p.EventDate, Outcome: outcome, Points: PredictionScoring.Points(outcome, rule));
                })
                .ToList();

            var settled = evaluated.Where(e => PredictionScoring.IsSettled(e.Outcome)).OrderBy(e => e.EventDate).ToList();

            var stats = new PredictionStatsDTO
            {
                TotalPredictions = evaluated.Count,
                Settled = settled.Count,
                Exact = settled.Count(e => e.Outcome == PredictionOutcome.Exact),
                Close = settled.Count(e => e.Outcome == PredictionOutcome.Close),
                Winner = settled.Count(e => e.Outcome == PredictionOutcome.Winner),
                Missed = settled.Count(e => e.Outcome == PredictionOutcome.Missed),
                Voided = evaluated.Count(e => e.Outcome == PredictionOutcome.Void),
                Pending = evaluated.Count(e => e.Outcome is PredictionOutcome.Pending or PredictionOutcome.Postponed)
            };

            // Bodovi zavise od sporta (SportScoringRules), pa se sabiraju po prognozi, a ne iz broja pogodaka.
            stats.Points = settled.Sum(e => e.Points);
            stats.SuccessRate = stats.Settled == 0 ? 0 : Math.Round(100.0 * (stats.Exact + stats.Close + stats.Winner) / stats.Settled, 1);

            var run = 0;
            foreach (var e in settled)
            {
                run = e.Points > 0 ? run + 1 : 0;
                stats.BestStreak = Math.Max(stats.BestStreak, run);
            }
            stats.CurrentStreak = run;

            stats.RecentForm = settled
                .TakeLast(10)
                .Select(e => e.Outcome switch
                {
                    PredictionOutcome.Exact => "E",
                    PredictionOutcome.Close => "C",
                    PredictionOutcome.Winner => "W",
                    _ => "M"
                })
                .ToList();

            var ranking = await BuildRankingAsync(await GetCircleUserIdsAsync(userId.Value, ct), ct);
            stats.RankedUsers = ranking.Count;
            var position = ranking.FindIndex(r => r.UserId == userId.Value);
            stats.Rank = position >= 0 ? position + 1 : null;

            return Ok(stats);
        }

        // Globalna rang-lista: top N + red ulogovanog korisnika (i kad nije u topu).
        [HttpGet("leaderboard")]
        public async Task<ActionResult<LeaderboardDTO>> GetLeaderboard([FromQuery] int take = 20, CancellationToken ct = default)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            take = Math.Clamp(take, 1, 100);

            var ranking = await BuildRankingAsync(await GetCircleUserIdsAsync(userId.Value, ct), ct);

            var wanted = ranking.Take(take).Select(r => r.UserId).ToHashSet();
            wanted.Add(userId.Value);

            var usernames = await _context.Users
                .AsNoTracking()
                .Where(u => wanted.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username, ct);

            LeaderboardEntryDTO ToEntry(int index) => new()
            {
                UserId = ranking[index].UserId,
                Rank = index + 1,
                Username = usernames.GetValueOrDefault(ranking[index].UserId, string.Empty),
                Points = ranking[index].Points,
                Exact = ranking[index].Exact,
                Settled = ranking[index].Settled,
                IsMe = ranking[index].UserId == userId.Value
            };

            var result = new LeaderboardDTO
            {
                Entries = Enumerable.Range(0, Math.Min(take, ranking.Count)).Select(ToEntry).ToList(),
                RankedUsers = ranking.Count,
            };

            var myIndex = ranking.FindIndex(r => r.UserId == userId.Value);
            if (myIndex >= 0)
                result.Me = ToEntry(myIndex);

            return Ok(result);
        }

        private sealed record RankedUser(long UserId, int Points, int Exact, int Settled);

        // Skup korisnika za rang-listu: ulogovani korisnik + njegovi PRIHVACENI prijatelji (Friendship je jedan red po
        // paru, korisnik moze biti sa obe strane veze).
        private async Task<List<long>> GetCircleUserIdsAsync(long userId, CancellationToken ct)
        {
            var friendIds = await _context.Friendships
                .AsNoTracking()
                .Where(f => f.Status == FriendshipStatus.Accepted && (f.RequesterId == userId || f.AddresseeId == userId))
                .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
                .ToListAsync(ct);

            friendIds.Add(userId);
            return friendIds.Distinct().ToList();
        }

        // Sortirana rang-lista za zadate korisnike - SVI iz skupa su u njoj (i oni bez zavrsene prognoze, sa 0 bodova),
        // da prijatelj bez prognoza ne nestane sa liste. Ucitavaju se samo prognoze za mecove koji MOGU biti zavrseni
        // (konacan status + rezultat), a ishod se racuna u memoriji istim PredictionScoring-om kao svuda.
        private async Task<List<RankedUser>> BuildRankingAsync(IReadOnlyCollection<long> userIds, CancellationToken ct)
        {
            var settledCodes = PredictionScoring.SettledStatusCodes;

            var rows = await _context.Predictions
                .AsNoTracking()
                .Where(p => userIds.Contains(p.UserId)
                    && p.Match.HomeTeamScore != null && p.Match.AwayTeamScore != null
                    && p.Match.Status != null && settledCodes.Contains(p.Match.Status))
                .Select(p => new
                {
                    p.UserId,
                    p.Match.Status,
                    p.Match.HomeTeamScore,
                    p.Match.AwayTeamScore,
                    SportId = p.Match.League.SportId,
                    p.PredictedHomeScore,
                    p.PredictedAwayScore
                })
                .ToListAsync(ct);

            var rules = await _context.LoadScoringRulesAsync(ct);

            var usernamesById = await _context.Users
                .AsNoTracking()
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username, ct);

            var perUser = rows
                .Select(r =>
                {
                    var rule = rules.For(r.SportId);
                    var outcome = PredictionScoring.Evaluate(r.Status, r.HomeTeamScore, r.AwayTeamScore, r.PredictedHomeScore, r.PredictedAwayScore, rule);
                    return (r.UserId, Outcome: outcome, Points: PredictionScoring.Points(outcome, rule));
                })
                .Where(r => PredictionScoring.IsSettled(r.Outcome))
                .GroupBy(r => r.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => new RankedUser(
                        g.Key,
                        g.Sum(r => r.Points),
                        g.Count(r => r.Outcome == PredictionOutcome.Exact),
                        g.Count()));

            return userIds
                .Distinct()
                .Select(id => perUser.GetValueOrDefault(id, new RankedUser(id, 0, 0, 0)))
                .OrderByDescending(r => r.Points)
                .ThenByDescending(r => r.Exact)
                .ThenBy(r => r.Settled)
                .ThenBy(r => usernamesById.GetValueOrDefault(r.UserId, string.Empty), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private long? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var id) ? id : null;
        }
    }
}
