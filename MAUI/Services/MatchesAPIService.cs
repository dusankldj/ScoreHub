using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MAUI.Services
{
    public class MatchesAPIService : IMatchesAPIService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public MatchesAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<List<MatchesDTO>> GetMatchesAsync(long? sportId, DateTime fromUtc, DateTime toUtc, string? status = null, CancellationToken ct = default)
        {
            var url = $"matches?from={Uri.EscapeDataString(fromUtc.ToString("o"))}&to={Uri.EscapeDataString(toUtc.ToString("o"))}";

            if (sportId.HasValue)
                url += $"&sportId={sportId.Value}";

            if (!string.IsNullOrEmpty(status))
                url += $"&status={Uri.EscapeDataString(status)}";

            var result = await _httpClient.GetFromJsonAsync<List<MatchesDTO>>(url, ct);
            return result ?? new List<MatchesDTO>();
        }
        #endregion
    }
}
