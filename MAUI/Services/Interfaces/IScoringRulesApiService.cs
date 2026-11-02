using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface IScoringRulesApiService
    {
        Task<List<SportScoringDTO>> GetRulesAsync(CancellationToken ct = default);
    }
}
