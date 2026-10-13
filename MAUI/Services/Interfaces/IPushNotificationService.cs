namespace MAUI.Services.Interfaces
{
    public interface IPushNotificationService
    {
        Task<bool> RequestPermissionAsync();

        Task<string?> GetTokenAsync();

        // Token ume da se promeni (reinstall, cache clear, itd) dok je korisnik vec ulogovan -
        // pretplatnik treba da ponovo registruje NOVI token na backend-u.
        event EventHandler<string>? TokenRefreshed;
    }
}
