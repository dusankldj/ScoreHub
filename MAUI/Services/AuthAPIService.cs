using MAUI.DTO.Input;
using MAUI.DTO.Output;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class AuthApiService : IAuthApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", request, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>(ct);
            return result ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        public async Task RegisterAsync(RegisterRequestDTO request, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", request, ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task<AuthResponseDTO> RefreshAsync(RefreshRequestDTO request, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/refresh", request, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>(ct);
            return result ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        public async Task LogoutAsync(CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsync("auth/logout", null, ct);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
