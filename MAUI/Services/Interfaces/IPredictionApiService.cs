using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface IPredictionApiService
    {
        Task<List<MyPredictionDTO>> GetMyPredictionsAsync(CancellationToken ct = default);

        Task<PredictionDTO?> GetMyPredictionAsync(long matchId, CancellationToken ct = default);

        Task SavePredictionAsync(long matchId, int predictedHomeScore, int predictedAwayScore, CancellationToken ct = default);
    }
}
