using MAUI.DTO.Input;
using MAUI.DTO.Output;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class SearchHistoryApiService : ISearchHistoryApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public SearchHistoryApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<List<RecentSearchDTO>> GetRecentSearchesAsync(CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<RecentSearchDTO>>("users/me/searches", ct);
            return result ?? new List<RecentSearchDTO>();
        }

        public async Task RecordSearchAsync(string kind, long entityId, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("users/me/searches", new RecordSearchRequestDTO { Kind = kind, EntityId = entityId }, ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveSearchAsync(long id, CancellationToken ct = default)
        {
            var response = await _httpClient.DeleteAsync($"users/me/searches/{id}", ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task ClearSearchesAsync(CancellationToken ct = default)
        {
            var response = await _httpClient.DeleteAsync("users/me/searches", ct);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
