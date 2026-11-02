using System.Collections.Concurrent;

namespace WebAPI.Helpers
{
    // Zastita od pogadjanja lozinke na endpoint-ima koji traze TRENUTNU lozinku (promena lozinke, promena e-maila): najvise
    // MaxAttempts POGRESNIH unosa u Window, posle toga 429 dok prozor ne istekne. U memoriji (jedna instanca servera) -
    // restart ga resetuje. Brojac je zajednicki za sve takve endpoint-e (isti napadac, isti ulog).
    public static class PasswordAttemptLimiter
    {
        public const int MaxAttempts = 5;
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(15);
        private static readonly ConcurrentDictionary<long, (int Failures, DateTime WindowStart)> Failures = new();

        public static bool IsLockedOut(long userId)
        {
            if (!Failures.TryGetValue(userId, out var entry))
                return false;

            if (DateTime.UtcNow - entry.WindowStart > Window)
            {
                Failures.TryRemove(userId, out _);
                return false;
            }

            return entry.Failures >= MaxAttempts;
        }

        public static void RegisterFailure(long userId)
        {
            var now = DateTime.UtcNow;
            Failures.AddOrUpdate(
                userId,
                _ => (1, now),
                (_, old) => now - old.WindowStart > Window ? (1, now) : (old.Failures + 1, old.WindowStart));
        }

        public static void Reset(long userId) => Failures.TryRemove(userId, out _);
    }
}
