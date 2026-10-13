using DataAccess;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Sync
{

    public class PlayerSyncBackgroundService : BackgroundService
    {
        #region Fields
        // 03:00 UTC - kasno nakon evropskih večernjih mečeva, dovoljno rano da ne smeta jutru.
        private static readonly TimeSpan DailyRunTimeUtc = new TimeSpan(3, 0, 0);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PlayerSyncBackgroundService> _logger;
        #endregion

        #region Constructor
        public PlayerSyncBackgroundService(IServiceScopeFactory scopeFactory, ILogger<PlayerSyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        #endregion

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun(DailyRunTimeUtc);
                _logger.LogInformation("Sledeci SyncPlayersAsync zakazan za {NextRunUtc:u} (za {Delay}).", DateTime.UtcNow.Add(delay), delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break; 
                }

                await RunSyncAsync(stoppingToken);
            }
        }

        private static TimeSpan GetDelayUntilNextRun(TimeSpan targetTimeUtc)
        {
            var now = DateTime.UtcNow;
            var todayTarget = now.Date.Add(targetTimeUtc);
            var nextRun = now <= todayTarget ? todayTarget : todayTarget.AddDays(1);

            return nextRun - now;
        }

        private async Task RunSyncAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var syncService = scope.ServiceProvider.GetRequiredService<ISportsDbSyncService>();
            var context = scope.ServiceProvider.GetRequiredService<ScoreHubDbContext>();

            try
            {
                _logger.LogInformation("Pokrećem dnevni SyncPlayersAsync.");
                await syncService.SyncPlayersAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncPlayersAsync (pozadinski) nije uspeo. Probaćemo opet sledeći ciklus.");
                await LogErrorAsync(context, "Background SyncPlayersAsync failed.", ex, ct);
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
