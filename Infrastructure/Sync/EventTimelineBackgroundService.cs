using DataAccess;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Sync
{
    public class EventTimelineBackgroundService : BackgroundService
    {
        #region Fields
        // Kratak tik NAMERNO - SyncRecentEventTimelineAsync sam odlucuje koji su mecevi "na redu" (po
        // TimelineSyncedAt i starosti meca) i ogranicava broj po ciklusu, pa prazan tik kosta jedan brz upit.
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(15);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventTimelineBackgroundService> _logger;
        #endregion

        #region Constructor
        public EventTimelineBackgroundService(IServiceScopeFactory scopeFactory, ILogger<EventTimelineBackgroundService> logger)
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
                await RunSyncAsync(stoppingToken);
            }
        }

        private async Task RunSyncAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var syncService = scope.ServiceProvider.GetRequiredService<ISportsDbSyncService>();
            var context = scope.ServiceProvider.GetRequiredService<ScoreHubDbContext>();

            try
            {
                await syncService.SyncRecentEventTimelineAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncRecentEventTimelineAsync (pozadinski) nije uspeo. Probaćemo opet sledeći ciklus (za ~15 min).");
                await LogErrorAsync(context, "Background SyncRecentEventTimelineAsync failed.", ex, ct);
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
