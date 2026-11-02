using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class FriendApiService : IFriendApiService
    {
        private readonly HttpClient _httpClient;

        public FriendApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FriendsPageDTO> GetFriendsAsync(int skip, int take, CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<FriendsPageDTO>($"friends?skip={skip}&take={take}", ct);
            return result ?? new FriendsPageDTO();
        }

        public async Task<List<FriendRequestDTO>> GetIncomingRequestsAsync(CancellationToken ct = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<FriendRequestDTO>>("friends/requests", ct);
            return result ?? new List<FriendRequestDTO>();
        }

        public Task<string> SendRequestAsync(long userId, CancellationToken ct = default)
            => SendAsync(HttpMethod.Post, $"friends/{userId}/request", ct);

        public Task<string> CancelRequestAsync(long userId, CancellationToken ct = default)
            => SendAsync(HttpMethod.Delete, $"friends/{userId}/request", ct);

        public Task<string> AcceptRequestAsync(long userId, CancellationToken ct = default)
            => SendAsync(HttpMethod.Post, $"friends/{userId}/accept", ct);

        public Task<string> DeclineRequestAsync(long userId, CancellationToken ct = default)
            => SendAsync(HttpMethod.Post, $"friends/{userId}/decline", ct);

        public Task<string> RemoveFriendAsync(long userId, CancellationToken ct = default)
            => SendAsync(HttpMethod.Delete, $"friends/{userId}", ct);

        private async Task<string> SendAsync(HttpMethod method, string url, CancellationToken ct)
        {
            using var request = new HttpRequestMessage(method, url);
            using var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<RelationshipDTO>(ct);
            return result?.Relationship ?? RelationshipState.None;
        }
    }
}
