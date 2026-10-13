using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Services
{
    public class SportsCacheService : ISportCacheService
    {
        #region Fields
        private readonly ISportsApiService _apiService;
        private List<SportsDTO>? _cache;
        #endregion

        #region Constructor
        public SportsCacheService(ISportsApiService apiService)
        {
            _apiService = apiService;
        }
        #endregion

        #region Methods
        public async Task<List<SportsDTO>> GetSportsAsync(CancellationToken ct = default)
        {
            if (_cache is not null)
                return _cache;

            _cache = await _apiService.GetSportsAsync(ct);
            return _cache;
        }
        #endregion
    }
}
