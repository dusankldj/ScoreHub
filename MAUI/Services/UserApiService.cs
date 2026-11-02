using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class UserApiService : IUserApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<UserProfileDTO> GetMeAsync(CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync("users/me", ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<UserProfileDTO>(ct);
            return result ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        public async Task<List<FavoriteTeamDTO>> GetFavoriteTeamsAsync(CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<FavoriteTeamDTO>>("users/me/favorite-teams", ct);
            return result ?? new List<FavoriteTeamDTO>();
        }

        public async Task<List<UserSearchResultDTO>> SearchUsersAsync(string query, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<UserSearchResultDTO>>($"users/search?query={Uri.EscapeDataString(query)}", ct);
            return result ?? new List<UserSearchResultDTO>();
        }

        public async Task<UserPageDTO> GetUserAsync(long userId, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<UserPageDTO>($"users/{userId}", ct);
            return result ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        public async Task<List<FavoriteTeamDTO>> GetUserFavoriteTeamsAsync(long userId, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<FavoriteTeamDTO>>($"users/{userId}/favorite-teams", ct);
            return result ?? new List<FavoriteTeamDTO>();
        }
        #endregion
    }
}
