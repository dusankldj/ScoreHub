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
    }
}
