using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class SportsApiService : ISportsApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Construtor
        public SportsApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<List<SportsDTO>> GetSportsAsync(CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<SportsDTO>>("Sports", ct);

            return result ?? new List<SportsDTO>();
        }
        #endregion

    }
}
