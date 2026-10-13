using DataAccess;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Sync
{
    public class MatchScheduleBackgroundService : BackgroundService
    {
        #region Fields
        private static readonly TimeSpan Interval = TimeSpan.FromHours(6);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MatchScheduleBackgroundService> _logger;
        #endregion

        #region Constructor
        public MatchScheduleBackgroundService(IServiceScopeFactory scopeFactory, ILogger<MatchScheduleBackgroundService> logger)
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
                _logger.LogInformation("Pokrećem SyncMatchesAsync (raspored, 6h ciklus).");
                await syncService.SyncMatchesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncMatchesAsync (pozadinski) nije uspeo. Probaćemo opet sledeći ciklus.");
                await LogErrorAsync(context, "Background SyncMatchesAsync failed.", ex, ct);
            }

            // Standings se inace osvezava SAMO reaktivno (kad live-tracker uhvati mec KOJI BAS
            // U TOM CIKLUSU prelazi u zavrseno stanje) - ako je WebAPI bio ugasen dok se mec
            // odigrao, taj "bas sad zavrsio" trenutak je zauvek propusten i tabela ostaje
            // zastarela. Ovaj periodicni prolaz kroz SVE lige (isti 6h ciklus kao raspored) je
            // "samoisceljujuci" safety-net nezavisan od tog reaktivnog puta.
            try
            {
                _logger.LogInformation("Pokrećem SyncStandingsAsync (raspored, 6h ciklus).");
                await syncService.SyncStandingsAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncStandingsAsync (pozadinski) nije uspeo. Probaćemo opet sledeći ciklus.");
                await LogErrorAsync(context, "Background SyncStandingsAsync failed.", ex, ct);
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
