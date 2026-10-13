using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Sync
{
    /// Sport/League/Team/Venue ("Celina 1") - retko se menjaju, sinhronizuju se ISKLJUČIVO:
    ///   1. Automatski u off-season periodu (druga polovina jula, 20.-31.7.) - jednom po prozoru.
    ///   2. Ručno, bilo kad, preko POST /api/admin/sync/all (AdminSyncController).
    ///
    /// NEMA sync-a pri startu aplikacije (namerno uklonjeno) - restart app-a tokom razvoja/testiranja
    /// vise ne izaziva dug sync.
    public class SportsDbStartupSyncService : BackgroundService
    {
        #region Fields
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(24);
        private const int OffSeasonMonth = 7;
        private const int OffSeasonFromDay = 20;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SportsDbStartupSyncService> _logger;
        #endregion

        #region Constructor
        public SportsDbStartupSyncService(IServiceScopeFactory scopeFactory, ILogger<SportsDbStartupSyncService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(CheckInterval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunOffSeasonCheckAsync(stoppingToken);
            }
        }

        private async Task RunOffSeasonCheckAsync(CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            if (!IsInOffSeasonWindow(now))
            {
                return; 
            }

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ScoreHubDbContext>();
            var syncService = scope.ServiceProvider.GetRequiredService<ISportsDbSyncService>();

            try
            {
                var lastSync = await context.Leagues.MaxAsync(x => (DateTime?)x.LastSyncedAt, ct);

                if (AlreadySyncedInThisOffSeasonWindow(lastSync, now))
                {
                    _logger.LogInformation(
                        "Već sinhronizovano u ovom off-season prozoru (poslednji sync: {LastSync}) - preskačem.",
                        lastSync);
                    return;
                }

                _logger.LogInformation("Ušli smo u off-season prozor (druga polovina jula) - pokrećem SyncAllAsync.");
                await syncService.SyncAllAsync(ct);

                _logger.LogInformation("Off-season prozor - pokrećem i SyncPlayerStatisticsAsync/SyncPlayerHonoursAsync/SyncPlayerMilestonesAsync (isti ritam).");
                await syncService.SyncPlayerStatisticsAsync(ct);
                await syncService.SyncPlayerHonoursAsync(ct);
                await syncService.SyncPlayerMilestonesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Zakazan (off-season) SyncAllAsync nije uspeo. Probaćemo opet sledeću dnevnu proveru.");
                await LogErrorAsync(context, "Scheduled off-season SyncAllAsync failed.", ex, ct);
            }
        }

        private static bool IsInOffSeasonWindow(DateTime utcNow)
            => utcNow.Month == OffSeasonMonth && utcNow.Day >= OffSeasonFromDay;

        private static bool AlreadySyncedInThisOffSeasonWindow(DateTime? lastSync, DateTime utcNow)
        {
            if (!lastSync.HasValue)
                return false;

            return lastSync.Value.Year == utcNow.Year
                && lastSync.Value.Month == OffSeasonMonth
                && lastSync.Value.Day >= OffSeasonFromDay;
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
    }
}
