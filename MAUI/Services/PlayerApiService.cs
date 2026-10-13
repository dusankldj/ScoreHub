using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class PlayerApiService : IPlayerApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public PlayerApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<PlayerDTO> GetPlayerDetailsAsync(long playerId, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<PlayerDTO>($"player/{playerId}", ct);
            return result ?? throw new InvalidOperationException("Player details response was empty.");
        }

        public async Task<List<PlayerSearchResultDTO>> SearchPlayersAsync(string query, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<PlayerSearchResultDTO>>($"player/search?query={Uri.EscapeDataString(query)}", ct);
            return result ?? new List<PlayerSearchResultDTO>();
        }
        #endregion
    }
}
