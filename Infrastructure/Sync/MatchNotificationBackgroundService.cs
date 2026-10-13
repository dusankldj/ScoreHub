using DataAccess;
using Domain.Entities;
using Infrastructure.Push;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Sync
{
    public class MatchNotificationBackgroundService : BackgroundService
    {
        #region Fields
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MatchNotificationBackgroundService> _logger;
        #endregion

        #region Constructor
        public MatchNotificationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<MatchNotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        #endregion

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunAsync(stoppingToken);
            }
        }

        private async Task RunAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ScoreHubDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<IMatchNotificationService>();

            try
            {
                var now = DateTime.UtcNow;


                var candidates = await context.Matches
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .Where(m => m.EventDate > now)
                    .Where(m => (m.Reminder24hSentAt == null && m.EventDate <= now.AddHours(24))
                             || (m.Reminder1hSentAt == null && m.EventDate <= now.AddHours(1)))
                    .Where(m => context.FavoriteTeams.Any(f => f.Notifications && (f.TeamId == m.HomeTeamId || f.TeamId == m.AwayTeamId)))
                    .ToListAsync(ct);

                _logger.LogInformation("MatchNotificationBackgroundService ciklus - {Count} kandidata za podsetnik.", candidates.Count);

                foreach (var match in candidates)
                {
                    try
                    {
                        if (match.Reminder24hSentAt == null && match.EventDate <= now.AddHours(24))
                        {
                            await notificationService.SendMatchReminderAsync(match, ct);
                            match.Reminder24hSentAt = now;
                        }

                        if (match.Reminder1hSentAt == null && match.EventDate <= now.AddHours(1))
                        {
                            await notificationService.SendMatchReminderAsync(match, ct);
                            match.Reminder1hSentAt = now;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Slanje podsetnika nije uspelo za Match Id={MatchId}.", match.Id);
                        await LogErrorAsync(context, $"Failed to send match reminder for Match Id={match.Id}.", ex, ct);
                    }
                }

                await context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MatchNotificationBackgroundService ciklus nije uspeo. Probacemo opet za ~5 min.");
                await LogErrorAsync(context, "Background MatchNotificationBackgroundService cycle failed.", ex, ct);
            }
        }

        private async Task LogErrorAsync(ScoreHubDbContext context, string englishContext, Exception ex, CancellationToken ct)
        {
            try
            {
                context.Logs.Add(new Log
                {
                    Exception = $"{englishContext} {ex.Message}",
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
                await context.SaveChangesAsync(ct);
            }
            catch (Exception logEx)
            {
                _logger.LogCritical(logEx, "Failed to write error log to database. Original error: {Context}", englishContext);
            }
        }
        #endregion
    }
}
