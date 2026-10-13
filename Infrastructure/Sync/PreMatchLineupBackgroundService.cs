using DataAccess;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Sync
{
    public class PreMatchLineupBackgroundService : BackgroundService
    {
        #region Fields
        private static readonly TimeSpan Interval = TimeSpan.FromHours(2);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PreMatchLineupBackgroundService> _logger;
        #endregion

        #region Constructor
        public PreMatchLineupBackgroundService(IServiceScopeFactory scopeFactory, ILogger<PreMatchLineupBackgroundService> logger)
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
                await syncService.SyncUpcomingLineupsAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncUpcomingLineupsAsync (pozadinski) nije uspeo. Probaćemo opet sledeći ciklus (za ~2h).");
                await LogErrorAsync(context, "Background SyncUpcomingLineupsAsync failed.", ex, ct);
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
