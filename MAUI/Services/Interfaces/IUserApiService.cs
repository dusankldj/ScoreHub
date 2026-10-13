using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface IUserApiService
    {
        Task<UserProfileDTO> GetMeAsync(CancellationToken ct = default);
        Task<List<FavoriteTeamDTO>> GetFavoriteTeamsAsync(CancellationToken ct = default);
    }
}
