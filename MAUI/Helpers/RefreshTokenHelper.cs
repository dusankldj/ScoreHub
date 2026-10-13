using MAUI.DTO.Input;
using MAUI.DTO.Output;
using MAUI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace MAUI.Helpers
{
    public class RefreshTokenHelper : DelegatingHandler
    {
        #region Fields
        private readonly IAuthService _authService;
        private readonly string _baseUrl;
        #endregion

        #region Constructor
        public RefreshTokenHelper(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _baseUrl = configuration["Api:BaseUrl"]
                ?? throw new InvalidOperationException("Api:BaseUrl not found in appsettings.json");
        }
        #endregion

        #region Methods
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _authService.GetTokenAsync();

            if (!string.IsNullOrEmpty(accessToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;
            
            if (string.IsNullOrEmpty(accessToken))
                return response;

            var refreshToken = await _authService.GetRefreshTokenAsync();

            if (string.IsNullOrEmpty(refreshToken))
            {
                await HandleRefreshFailureAsync();
                return response;
            }

            var refreshSucceeded = await TryRefreshTokenAsync(accessToken, refreshToken, cancellationToken);

            if (!refreshSucceeded)
            {
                await HandleRefreshFailureAsync();
                return response;
            }

            
            var newAccessToken = await _authService.GetTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<bool> TryRefreshTokenAsync(string oldToken, string refreshToken, CancellationToken ct)
        {
            try
            {
                using var plainClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };

                var response = await plainClient.PostAsJsonAsync("auth/refresh", new RefreshRequestDTO
                {
                    Token = oldToken,
                    RefreshToken = refreshToken
                }, ct);

                if (!response.IsSuccessStatusCode)
                    return false;

                var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>(ct);
                if (result is null)
                    return false;

                await _authService.SaveTokensAsync(result.Token, result.RefreshToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task HandleRefreshFailureAsync()
        {
            await _authService.LogoutAsync();


            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync("login");
            });
        }
        #endregion
    }
}

