using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Services
{
    public class UserCacheService : IUserCacheService
    {
        #region Fields
        private readonly IUserApiService _apiService;
        private UserProfileDTO? _cache;
        #endregion

        #region Constructor
        public UserCacheService(IUserApiService apiService)
        {
            _apiService = apiService;
        }
        #endregion

        #region Methods
        public void Clear()
        {
            this._cache = null;
        }

        public async Task<UserProfileDTO> GetMeAsync(CancellationToken ct = default)
        {
            if (_cache is not null)
                return _cache;

            _cache = await _apiService.GetMeAsync(ct);
            return _cache;
        }
        #endregion
    }
}
