using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Push
{
    public class MatchNotificationService : IMatchNotificationService
    {
        private readonly ScoreHubDbContext _context;
        private readonly IPushNotificationService _pushService;

        public MatchNotificationService(ScoreHubDbContext context, IPushNotificationService pushService)
        {
            _context = context;
            _pushService = pushService;
        }

        private static readonly TimeZoneInfo DisplayTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Belgrade");

        public async Task SendMatchReminderAsync(Match match, CancellationToken ct = default)
        {
            var (homeUserIds, awayOnlyUserIds) = await GetFavoriterGroupsAsync(match, ct);
            if (homeUserIds.Count == 0 && awayOnlyUserIds.Count == 0)
                return;

            var utcNow = DateTime.UtcNow;
            var localDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(match.EventDate, DateTimeKind.Utc), DisplayTimeZone);
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, DisplayTimeZone);
            var when = localDate.Date == localNow.Date ? "Today" : localDate.Date == localNow.Date.AddDays(1) ? "Tomorrow" : localDate.ToString("dd/MM");
            const string title = "Upcoming match";

            var body = $"{match.HomeTeam.Name} vs {match.AwayTeam.Name} - {when} {localDate:HH:mm}";
            var data = new Dictionary<string, string>
            {
                ["eventId"] = match.Id.ToString(),
                ["type"] = "reminder",
                ["eventDateUtc"] = DateTime.SpecifyKind(match.EventDate, DateTimeKind.Utc).ToString("o"),
                ["homeTeamName"] = match.HomeTeam.Name,
                ["awayTeamName"] = match.AwayTeam.Name
            };


            await SendToGroupAsync(homeUserIds, title, body, data, match.HomeTeam.LogoUrl, ct);
            await SendToGroupAsync(awayOnlyUserIds, title, body, data, match.AwayTeam.LogoUrl, ct);
        }

        public async Task SendGoalNotificationAsync(Match match, bool isHomeGoal, CancellationToken ct = default)
        {
            var tokens = await GetAllFavoriterTokensAsync(match, ct);
            if (tokens.Count == 0)
                return;

            var scoringTeam = isHomeGoal ? match.HomeTeam : match.AwayTeam;
            var title = $"GOAL! {scoringTeam.Name}";
            var body = $"{match.HomeTeam.Name} {match.HomeTeamScore}-{match.AwayTeamScore} {match.AwayTeam.Name}";
            var data = new Dictionary<string, string> { ["eventId"] = match.Id.ToString(), ["type"] = "goal" };

            await _pushService.SendAsync(tokens, title, body, data, scoringTeam.LogoUrl, ct);
        }

        // Korisnici koji su prognozirali ovaj mec dobijaju PERSONALIZOVANU verziju (rezultat +
        // "You hit!/You missed" + njihova prognoza) UMESTO obicne - inace bi favoriter koji je
        // i prognozirao dobio dve notifikacije za isti kraj meca.
        public async Task SendFinalScoreNotificationAsync(Match match, CancellationToken ct = default)
        {
            const string title = "Full time";
            var scoreLine = $"{match.HomeTeam.Name} {match.HomeTeamScore}-{match.AwayTeamScore} {match.AwayTeam.Name}";

            var predictions = new List<Prediction>();
            if (match.HomeTeamScore is int finalHome && match.AwayTeamScore is int finalAway)
            {
                predictions = await _context.Predictions.Where(p => p.MatchId == match.Id).ToListAsync(ct);

                foreach (var prediction in predictions)
                    prediction.Hit = prediction.PredictedHomeScore == finalHome && prediction.PredictedAwayScore == finalAway;

                if (predictions.Count > 0)
                    await _context.SaveChangesAsync(ct);
            }

            var predictorIds = predictions.Select(p => p.UserId).ToHashSet();

            var genericUserIds = (await GetFavoriterUserIdsAsync(match, ct)).Where(id => !predictorIds.Contains(id)).ToList();
            var genericTokens = await GetTokensForUsersAsync(genericUserIds, ct);
            if (genericTokens.Count > 0)
            {
                var data = new Dictionary<string, string> { ["eventId"] = match.Id.ToString(), ["type"] = "finalScore" };
                await _pushService.SendAsync(genericTokens, title, scoreLine, data, match.HomeTeam.LogoUrl, ct);
            }

            // Isti par (prognoza, pogodak) deli istu poruku - jedan poziv po grupi, ne po korisniku.
            foreach (var group in predictions.GroupBy(p => (p.PredictedHomeScore, p.PredictedAwayScore, p.Hit)))
            {
                var tokens = await GetTokensForUsersAsync(group.Select(p => p.UserId).ToList(), ct);
                if (tokens.Count == 0)
                    continue;

                var (predictedHome, predictedAway, hit) = group.Key;
                var verdict = hit ? "You hit!" : "You missed";
                var predictionLine = $"Your prediction: {predictedHome}-{predictedAway}";

                var data = new Dictionary<string, string>
                {
                    ["eventId"] = match.Id.ToString(),
                    ["type"] = "predictionResult",
                    ["scoreLine"] = scoreLine,
                    ["verdict"] = verdict,
                    ["predictionLine"] = predictionLine,
                    ["hit"] = hit ? "true" : "false"
                };

                await _pushService.SendAsync(tokens, title, $"{scoreLine}\n{verdict}\n{predictionLine}", data, match.HomeTeam.LogoUrl, ct);
            }
        }

        private async Task SendToGroupAsync(HashSet<long> userIds, string title, string body, IDictionary<string, string> data, string? imageUrl, CancellationToken ct)
        {
            if (userIds.Count == 0)
                return;

            var tokens = await _context.DeviceTokens.Where(d => userIds.Contains(d.UserId)).Select(d => d.Token).ToListAsync(ct);
            await _pushService.SendAsync(tokens, title, body, data, imageUrl, ct);
        }

        private async Task<List<string>> GetAllFavoriterTokensAsync(Match match, CancellationToken ct)
            => await GetTokensForUsersAsync(await GetFavoriterUserIdsAsync(match, ct), ct);

        private async Task<List<long>> GetFavoriterUserIdsAsync(Match match, CancellationToken ct)
            => await _context.FavoriteTeams
                .Where(f => f.Notifications && (f.TeamId == match.HomeTeamId || f.TeamId == match.AwayTeamId))
                .Select(f => f.UserId)
                .Distinct()
                .ToListAsync(ct);

        private async Task<List<string>> GetTokensForUsersAsync(List<long> userIds, CancellationToken ct)
        {
            if (userIds.Count == 0)
                return new List<string>();

            return await _context.DeviceTokens.Where(d => userIds.Contains(d.UserId)).Select(d => d.Token).ToListAsync(ct);
        }


        private async Task<(HashSet<long> HomeUserIds, HashSet<long> AwayOnlyUserIds)> GetFavoriterGroupsAsync(Match match, CancellationToken ct)
        {
            var favorites = await _context.FavoriteTeams
                .Where(f => f.Notifications && (f.TeamId == match.HomeTeamId || f.TeamId == match.AwayTeamId))
                .Select(f => new { f.UserId, f.TeamId })
                .ToListAsync(ct);

            var homeUserIds = favorites.Where(f => f.TeamId == match.HomeTeamId).Select(f => f.UserId).ToHashSet();
            var awayOnlyUserIds = favorites.Where(f => f.TeamId == match.AwayTeamId).Select(f => f.UserId)
                .Where(id => !homeUserIds.Contains(id)).ToHashSet();

            return (homeUserIds, awayOnlyUserIds);
        }
    }
}
