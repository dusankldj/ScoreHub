using DataAccess;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Sync
{
    /// <summary>
    /// Istorija transfera igrača - retko se menja (par transfera godišnje po igraču),
    /// dovoljno je osvežavati na 6 meseci. Metoda sama unutar sebe ima 6-mesečni filter
    /// po igraču (FormerTeamsSyncedAt), pa je bezbedno pozivati je i češće nego što tajmer
    /// nalaže - jednostavno neće imati šta da radi ako je sve već sveže.
    /// </summary>
    public class PlayerFormerTeamBackgroundService : BackgroundService
    {
        #region Fields
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PlayerFormerTeamBackgroundService> _logger;
        #endregion

        #region Constructor
        public PlayerFormerTeamBackgroundService(IServiceScopeFactory scopeFactory, ILogger<PlayerFormerTeamBackgroundService> logger)
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
                _logger.LogInformation("Pokrećem SyncPlayerFormerTeamsAsync (dnevna provera, interni 6-mesečni filter po igraču).");
                await syncService.SyncPlayerFormerTeamsAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncPlayerFormerTeamsAsync (pozadinski) nije uspeo. Probaćemo opet sledeći ciklus.");
                await LogErrorAsync(context, "Background SyncPlayerFormerTeamsAsync failed.", ex, ct);
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
