using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface ITeamApiService
    {
        Task<TeamDTO> GetTeamDetailsAsync(long teamId, CancellationToken ct = default);
        Task<List<TeamSearchResultDTO>> SearchTeamsAsync(string query, CancellationToken ct = default);
        Task<bool> IsTeamFavoritedAsync(long teamId, CancellationToken ct = default);
        Task AddFavoriteTeamAsync(long teamId, CancellationToken ct = default);
        Task RemoveFavoriteTeamAsync(long teamId, CancellationToken ct = default);
    }
}
