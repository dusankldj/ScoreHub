using MAUI.DTO.Input;
using MAUI.DTO.Output;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class AccountApiService : IAccountApiService
    {
        private readonly HttpClient _httpClient;

        public AccountApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UpdateUsernameAsync(string username, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync("users/me/username", new UpdateUsernameRequestDTO { Username = username }, ct);
            return (await ReadAsync<UsernameDTO>(response, ct)).Username;
        }

        public async Task<CodeSentDTO> RequestEmailChangeAsync(string newEmail, string currentPassword, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("users/me/email/request",
                new RequestEmailChangeDTO { NewEmail = newEmail, CurrentPassword = currentPassword }, ct);
            return await ReadAsync<CodeSentDTO>(response, ct);
        }

        public async Task<CodeSentDTO> ResendEmailCodeAsync(CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsync("users/me/email/resend", null, ct);
            return await ReadAsync<CodeSentDTO>(response, ct);
        }

        public async Task DeleteAccountAsync(CancellationToken ct = default)
        {
            var response = await _httpClient.DeleteAsync("users/me", ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Could not delete your account. Please try again.");
        }

        public async Task<string> ConfirmEmailChangeAsync(string code, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("users/me/email/confirm", new ConfirmEmailCodeDTO { Code = code }, ct);
            return (await ReadAsync<EmailDTO>(response, ct)).Email;
        }

        // Uspeh -> telo; greska -> InvalidOperationException sa porukom servera ({ "Errors": ["..."] }) ili opstom porukom.
        private static async Task<T> ReadAsync<T>(HttpResponseMessage response, CancellationToken ct)
        {
            if (!response.IsSuccessStatusCode)
            {
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

            var result = await response.Content.ReadFromJsonAsync<T>(ct);
            return result ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        private sealed class ServerErrors
        {
            public List<string>? Errors { get; set; }
        }
    }
}
