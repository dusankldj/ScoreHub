namespace Infrastructure.Push
{
    public interface IPushNotificationService
    {
        Task SendAsync(IEnumerable<string> deviceTokens, string title, string body, IDictionary<string, string>? data, string? imageUrl = null, CancellationToken ct = default);
    }
}
