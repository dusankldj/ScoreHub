using DataAccess;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Push
{
    public class FcmPushNotificationService : IPushNotificationService
    {
        private const int BatchSize = 500; 

        private readonly FirebaseMessaging? _messaging;
        private readonly ScoreHubDbContext _context;
        private readonly ILogger<FcmPushNotificationService> _logger;

        public FcmPushNotificationService(FirebaseMessaging? messaging, ScoreHubDbContext context, ILogger<FcmPushNotificationService> logger)
        {
            _messaging = messaging;
            _context = context;
            _logger = logger;
        }

        public async Task SendAsync(IEnumerable<string> deviceTokens, string title, string body, IDictionary<string, string>? data, string? imageUrl = null, CancellationToken ct = default)
        {
            var tokens = deviceTokens.Distinct().ToList();
            if (tokens.Count == 0)
                return;

            if (_messaging is null)
            {
                _logger.LogWarning("Push notifikacija PRESKOCENA (Firebase nije konfigurisan - nedostaje service-account JSON): '{Title}' za {Count} uredjaja.", title, tokens.Count);
                return;
            }

            var deadTokens = new List<string>();

            
            var fullData = data is null ? new Dictionary<string, string>() : new Dictionary<string, string>(data);
            fullData["title"] = title;
            fullData["body"] = body;
            if (imageUrl is not null)
                fullData["imageUrl"] = imageUrl;

            for (var i = 0; i < tokens.Count; i += BatchSize)
            {
                var batch = tokens.Skip(i).Take(BatchSize).ToList();

                var messages = batch.Select(token => new Message
                {
                    Token = token,
                    Data = fullData
                }).ToList();

                try
                {
                    var response = await _messaging.SendEachAsync(messages, ct);

                    _logger.LogInformation("FCM SendEachAsync: '{Title}' -> {Success}/{Total} uspesno.", title, response.SuccessCount, batch.Count);

                    for (var j = 0; j < response.Responses.Count; j++)
                    {
                        var result = response.Responses[j];
                        if (result.IsSuccess)
                            continue;

                        var errorCode = result.Exception?.MessagingErrorCode;
                        if (errorCode is MessagingErrorCode.Unregistered or MessagingErrorCode.InvalidArgument)
                        {
                            deadTokens.Add(batch[j]);
                        }
                        else
                        {
                            _logger.LogWarning("Slanje push notifikacije nije uspelo za jedan token: {Error}", result.Exception?.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SendEachAsync (FCM) nije uspeo za batch od {Count} tokena.", batch.Count);
                }
            }

            if (deadTokens.Count > 0)
            {
                await _context.DeviceTokens.Where(d => deadTokens.Contains(d.Token)).ExecuteDeleteAsync(ct);
            }
        }
    }
}
