using Kotlin.Contracts;
using MAUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Services
{
    public class AuthService : IAuthService
    {
        #region Fields
        private const string TokenKey = "token";
        private const string RefreshTokenKey = "refresh_token";
        #endregion

        #region Methods
        public async Task<string?> GetRefreshTokenAsync()
        {
            try
            {
                return await SecureStorage.Default.GetAsync(RefreshTokenKey);
            }
            catch 
            {
                return null;
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.Default.GetAsync(TokenKey);
            }
            catch 
            {
                return null;
            }
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await GetTokenAsync();

            return !string.IsNullOrWhiteSpace(token);
        }

        public Task LogoutAsync()
        {
            try
            {
                SecureStorage.Default.Remove(TokenKey);
                SecureStorage.Default.Remove(RefreshTokenKey);
            }
            catch
            {
                try { SecureStorage.Default.RemoveAll(); } catch { }
            }

            return Task.CompletedTask;
        }

        public async Task SaveTokensAsync(string token, string refreshToken)
        {
            await SecureStorage.SetAsync(TokenKey, token);
            await SecureStorage.SetAsync(RefreshTokenKey, refreshToken);
        }
        #endregion
    }
}
