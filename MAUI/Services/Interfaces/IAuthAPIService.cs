using MAUI.DTO.Input;
using MAUI.DTO.Output;

namespace MAUI.Services.Interfaces
{
    public interface IAuthApiService
    {
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken ct = default);
        Task RegisterAsync(RegisterRequestDTO request, CancellationToken ct = default);
        Task<AuthResponseDTO> RefreshAsync(RefreshRequestDTO request, CancellationToken ct = default);
        Task LogoutAsync(CancellationToken ct = default);

        // Forgot password (bez prijave): 1) kod na e-mail, 2) provera koda -> reset token, 3) nova lozinka. Pri gresci baca
        // InvalidOperationException sa porukom servera.
        Task<CodeSentDTO> ForgotPasswordAsync(string email, CancellationToken ct = default);

        Task<string> VerifyResetCodeAsync(string email, string code, CancellationToken ct = default);

        Task ResetPasswordAsync(string email, string resetToken, string newPassword, CancellationToken ct = default);

        // Vraca NOVE tokene (server rotira refresh token). Pri gresci baca InvalidOperationException sa porukom servera
        // (npr. "Current password is incorrect.").
        Task<AuthResponseDTO> ChangePasswordAsync(ChangePasswordRequestDTO request, CancellationToken ct = default);
    }
}
