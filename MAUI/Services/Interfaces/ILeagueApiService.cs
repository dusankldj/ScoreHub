using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface ILeagueApiService
    {
        Task<List<LeagueSearchResultDTO>> SearchLeaguesAsync(string query, CancellationToken ct = default);

        Task<LeagueDetailsDTO> GetLeagueDetailsAsync(long leagueId, CancellationToken ct = default);
    }
}
