using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class EventDetailsApiService : IEventDetailsApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public EventDetailsApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<EventDetailsDTO> GetEventDetailsAsync(long eventId, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<EventDetailsDTO>($"event/{eventId}", ct);
            return result ?? throw new InvalidOperationException("Event details response was empty.");
        }
        #endregion
    }
}
