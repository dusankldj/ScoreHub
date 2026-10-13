using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsLoggedInAsync();
        Task<string?> GetTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        Task SaveTokensAsync(string token, string refreshToken);
        Task LogoutAsync();
    }
}
