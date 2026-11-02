using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface IFriendApiService
    {
        // Server paging - prijatelji ulogovanog korisnika, poslednje dodati prvi.
        Task<FriendsPageDTO> GetFriendsAsync(int skip, int take, CancellationToken ct = default);

        Task<List<FriendRequestDTO>> GetIncomingRequestsAsync(CancellationToken ct = default);

        // Sve akcije vracaju NOVI odnos (Relationship.*) - klijent ga samo prikaze, ne izvodi sam.
        Task<string> SendRequestAsync(long userId, CancellationToken ct = default);

        Task<string> CancelRequestAsync(long userId, CancellationToken ct = default);

        Task<string> AcceptRequestAsync(long userId, CancellationToken ct = default);

        Task<string> DeclineRequestAsync(long userId, CancellationToken ct = default);

        Task<string> RemoveFriendAsync(long userId, CancellationToken ct = default);
    }
}
