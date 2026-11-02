using MAUI.DTO.Input;
using MAUI.DTO.Output;
using MAUI.Services.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class PredictionApiService : IPredictionApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public PredictionApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<List<MyPredictionDTO>> GetMyPredictionsAsync(CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<MyPredictionDTO>>("predictions/me", ct);
            return result ?? new List<MyPredictionDTO>();
        }

        public async Task<List<MyPredictionDTO>> GetUserPredictionsAsync(long userId, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<MyPredictionDTO>>($"predictions/user/{userId}", ct);
            return result ?? new List<MyPredictionDTO>();
        }

        public async Task<PredictionStatsDTO> GetMyStatsAsync(CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<PredictionStatsDTO>("predictions/me/stats", ct);
            return result ?? new PredictionStatsDTO();
        }

        public async Task<LeaderboardDTO> GetLeaderboardAsync(int take = 20, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<LeaderboardDTO>($"predictions/leaderboard?take={take}", ct);
            return result ?? new LeaderboardDTO();
        }

        // 404 znaci "korisnik jos nije prognozirao ovaj mec" - to je normalno stanje, ne greska.
        public async Task<PredictionDTO?> GetMyPredictionAsync(long matchId, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"predictions/match/{matchId}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PredictionDTO>(ct);
        }

        public async Task SavePredictionAsync(long matchId, int predictedHomeScore, int predictedAwayScore, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"predictions/match/{matchId}", new SavePredictionRequestDTO
            {
                PredictedHomeScore = predictedHomeScore,
                PredictedAwayScore = predictedAwayScore
            }, ct);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
