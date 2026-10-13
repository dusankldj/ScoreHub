namespace MAUI.Services.Interfaces
{
    public interface IDeviceTokenApiService
    {
        Task RegisterTokenAsync(string token, CancellationToken ct = default);

        Task UnregisterTokenAsync(string token, CancellationToken ct = default);
    }
}
