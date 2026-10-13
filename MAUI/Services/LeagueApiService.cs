using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class LeagueApiService : ILeagueApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public LeagueApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<List<LeagueSearchResultDTO>> SearchLeaguesAsync(string query, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<LeagueSearchResultDTO>>($"league/search?query={Uri.EscapeDataString(query)}", ct);
            return result ?? new List<LeagueSearchResultDTO>();
        }

        public async Task<LeagueDetailsDTO> GetLeagueDetailsAsync(long leagueId, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<LeagueDetailsDTO>($"league/{leagueId}", ct);
            return result ?? throw new InvalidOperationException("League details response was empty.");
        }
        #endregion
    }
}
