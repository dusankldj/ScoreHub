using System.Collections.Concurrent;

namespace WebAPI.Helpers
{
    // Ogranicenje broja zahteva po kljucu (IP adresa, e-mail...) u kliznom prozoru - za endpoint-e BEZ prijave (Forgot password),
    // gde ne postoji korisnik po kom bi se ogranicavalo. Racuna SVAKI zahtev, bez obzira da li nalog postoji, pa ne odaje
    // koji nalozi postoje. U memoriji (jedna instanca servera) - restart ga resetuje.
    public static class RequestThrottle
    {
        private static readonly ConcurrentDictionary<string, List<DateTime>> Hits = new();

        // true = zahtev je dozvoljen (i upisan); false = prekoracen limit u zadatom prozoru.
        public static bool Allow(string key, int maxRequests, TimeSpan window)
        {
            var now = DateTime.UtcNow;
            var list = Hits.GetOrAdd(key, _ => new List<DateTime>());

            lock (list)
            {
                list.RemoveAll(t => now - t > window);

                if (list.Count >= maxRequests)
                    return false;

                list.Add(now);
                return true;
            }
        }
    }
}
