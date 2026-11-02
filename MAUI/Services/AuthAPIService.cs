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

        public async Task<CodeSentDTO> ForgotPasswordAsync(string email, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/forgot-password", new ForgotPasswordRequestDTO { Email = email }, ct);
            await ThrowIfErrorAsync(response, ct);

            return await response.Content.ReadFromJsonAsync<CodeSentDTO>(ct)
                   ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        public async Task<string> VerifyResetCodeAsync(string email, string code, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/forgot-password/verify", new VerifyResetCodeRequestDTO { Email = email, Code = code }, ct);
            await ThrowIfErrorAsync(response, ct);

            var body = await response.Content.ReadFromJsonAsync<ResetTokenDTO>(ct);
            return body?.ResetToken ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        public async Task ResetPasswordAsync(string email, string resetToken, string newPassword, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/reset-password",
                new ResetPasswordRequestDTO { Email = email, ResetToken = resetToken, NewPassword = newPassword }, ct);
            await ThrowIfErrorAsync(response, ct);
        }

        // Uspeh -> nista; greska -> InvalidOperationException sa porukom servera ({ "Errors": ["..."] }) ili opstom porukom.
        private static async Task ThrowIfErrorAsync(HttpResponseMessage response, CancellationToken ct)
        {
            if (response.IsSuccessStatusCode)
                return;

            string? message = null;
            try
            {
                var body = await response.Content.ReadFromJsonAsync<ServerErrors>(ct);
                message = body?.Errors?.FirstOrDefault();
            }
            catch (Exception)
            {
                // telo nije u ocekivanom obliku - ide opsta poruka ispod
            }

            throw new InvalidOperationException(message ?? "Something went wrong. Please try again.");
        }

        public async Task<AuthResponseDTO> ChangePasswordAsync(ChangePasswordRequestDTO request, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/change-password", request, ct);

            if (!response.IsSuccessStatusCode)
            {
                // Server salje { "Errors": ["..."] } (netacna trenutna lozinka, ista nova lozinka, previse pokusaja).
                string? message = null;
                try
                {
                    var body = await response.Content.ReadFromJsonAsync<ServerErrors>(ct);
                    message = body?.Errors?.FirstOrDefault();
                }
                catch (Exception)
                {
                    // telo nije u ocekivanom obliku - ide opsta poruka ispod
                }

                throw new InvalidOperationException(message ?? "Could not change the password. Please try again.");
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>(ct);
            return result ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        private sealed class ServerErrors
        {
            public List<string>? Errors { get; set; }
        }

        public async Task LogoutAsync(CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsync("auth/logout", null, ct);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
