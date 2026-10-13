using MAUI.DTO.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Services.Interfaces
{
    public interface IMatchesAPIService
    {
        Task<List<MatchesDTO>> GetMatchesAsync(long? sportId, DateTime fromUtc, DateTime toUtc, string? status = null, CancellationToken ct = default);
    }
}
