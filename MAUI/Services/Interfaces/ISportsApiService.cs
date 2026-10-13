using MAUI.DTO.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Services.Interfaces
{
    public interface ISportsApiService
    {
        Task<List<SportsDTO>> GetSportsAsync(CancellationToken ct = default);
    }
}
