using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class TeamApiService : ITeamApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public TeamApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<TeamDTO> GetTeamDetailsAsync(long teamId, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<TeamDTO>($"team/{teamId}", ct);
            return result ?? throw new InvalidOperationException("Team details response was empty.");
        }

        public async Task<List<TeamSearchResultDTO>> SearchTeamsAsync(string query, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<TeamSearchResultDTO>>($"team/search?query={Uri.EscapeDataString(query)}", ct);
            return result ?? new List<TeamSearchResultDTO>();
        }

        public async Task<bool> IsTeamFavoritedAsync(long teamId, CancellationToken ct = default)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"team/{teamId}/favorite", ct);
        }

        public async Task AddFavoriteTeamAsync(long teamId, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsync($"team/{teamId}/favorite", null, ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveFavoriteTeamAsync(long teamId, CancellationToken ct = default)
        {
            var response = await _httpClient.DeleteAsync($"team/{teamId}/favorite", ct);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
