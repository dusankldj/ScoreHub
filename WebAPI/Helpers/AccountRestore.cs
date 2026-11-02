using Domain.Entities;

namespace WebAPI.Helpers
{
    // Rok u kome soft-delete-ovan nalog moze da se vrati prijavom (Account:RestoreGraceDays, podrazumevano 30 dana).
    // Posle roka prijava ostaje odbijena, a nalog ceka trajno brisanje (SQL skripta).
    public static class AccountRestore
    {
        private const int DefaultGraceDays = 30;

        public static int GraceDays(IConfiguration configuration)
        {
            var days = configuration.GetValue("Account:RestoreGraceDays", DefaultGraceDays);
            return days > 0 ? days : DefaultGraceDays;
        }

        // Nalozi obrisani pre kolone DeletedAt (null) racunaju se od poslednje izmene reda.
        public static bool IsWithinWindow(User user, int graceDays)
        {
            var deletedAt = user.DeletedAt ?? user.UpdatedAt ?? user.CreatedAt;
            return deletedAt.AddDays(graceDays) >= DateTime.UtcNow;
        }
    }
}
