using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    // Edit profile. Sve metode pri gresci bacaju InvalidOperationException sa porukom servera
    // (npr. "This username is already taken.").
    public interface IAccountApiService
    {
        Task<string> UpdateUsernameAsync(string username, CancellationToken ct = default);

        // Salje sestocifreni kod na NOVU adresu (uz trenutnu lozinku).
        Task<CodeSentDTO> RequestEmailChangeAsync(string newEmail, string currentPassword, CancellationToken ct = default);

        Task<CodeSentDTO> ResendEmailCodeAsync(CancellationToken ct = default);

        // Soft delete naloga na serveru (korisnik se posle toga mora lokalno odjaviti).
        Task DeleteAccountAsync(CancellationToken ct = default);

        // Vraca novu (potvrdjenu) adresu.
        Task<string> ConfirmEmailChangeAsync(string code, CancellationToken ct = default);
    }
}
