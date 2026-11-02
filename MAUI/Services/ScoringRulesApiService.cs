using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class ScoringRulesApiService : IScoringRulesApiService
    {
        private readonly HttpClient _httpClient;

        public ScoringRulesApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SportScoringDTO>> GetRulesAsync(CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<SportScoringDTO>>("scoring-rules", ct);
            return result ?? new List<SportScoringDTO>();
        }
    }
}
