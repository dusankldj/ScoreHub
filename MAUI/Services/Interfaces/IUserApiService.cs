using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface IUserApiService
    {
        Task<UserProfileDTO> GetMeAsync(CancellationToken ct = default);
        Task<List<FavoriteTeamDTO>> GetFavoriteTeamsAsync(CancellationToken ct = default);

        // Pretraga korisnika po korisnickom imenu (bez sebe, najvise 20).
        Task<List<UserSearchResultDTO>> SearchUsersAsync(string query, CancellationToken ct = default);

        Task<UserPageDTO> GetUserAsync(long userId, CancellationToken ct = default);

        // Samo za prijatelje - server za ostale vraca 403.
        Task<List<FavoriteTeamDTO>> GetUserFavoriteTeamsAsync(long userId, CancellationToken ct = default);
    }
}
