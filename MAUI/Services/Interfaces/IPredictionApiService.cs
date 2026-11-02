using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface IPredictionApiService
    {
        Task<List<MyPredictionDTO>> GetMyPredictionsAsync(CancellationToken ct = default);

        // Prognoze prijatelja (isti oblik kao "my"); server za nepriijatelje vraca 403.
        Task<List<MyPredictionDTO>> GetUserPredictionsAsync(long userId, CancellationToken ct = default);

        Task<PredictionStatsDTO> GetMyStatsAsync(CancellationToken ct = default);

        Task<LeaderboardDTO> GetLeaderboardAsync(int take = 20, CancellationToken ct = default);

        Task<PredictionDTO?> GetMyPredictionAsync(long matchId, CancellationToken ct = default);

        Task SavePredictionAsync(long matchId, int predictedHomeScore, int predictedAwayScore, CancellationToken ct = default);
    }
}
