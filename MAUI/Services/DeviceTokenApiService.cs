using MAUI.DTO.Input;
using MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace MAUI.Services
{
    public class DeviceTokenApiService : IDeviceTokenApiService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public DeviceTokenApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task RegisterTokenAsync(string token, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("users/me/device-token", new DeviceTokenDTO { Token = token }, ct);
            response.EnsureSuccessStatusCode();
        }

        // HttpClient.DeleteAsync ne prihvata telo - DELETE sa JSON telom trazi rucno sastavljen
        // HttpRequestMessage (isti razlog kao [FromBody] na DELETE endpoint-u na backend-u).
        public async Task UnregisterTokenAsync(string token, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "users/me/device-token")
            {
                Content = JsonContent.Create(new DeviceTokenDTO { Token = token })
            };

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
