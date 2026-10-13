using MAUI.DTO.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Services.Interfaces
{
    public interface IUserCacheService
    {
        Task<UserProfileDTO> GetMeAsync(CancellationToken ct = default);

        void Clear();
    }
}
