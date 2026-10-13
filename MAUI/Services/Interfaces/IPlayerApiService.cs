using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface IPlayerApiService
    {
        Task<PlayerDTO> GetPlayerDetailsAsync(long playerId, CancellationToken ct = default);
        Task<List<PlayerSearchResultDTO>> SearchPlayersAsync(string query, CancellationToken ct = default);
    }
}
