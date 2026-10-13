using MAUI.DTO.Input;

namespace MAUI.Services.Interfaces
{
    public interface ISearchHistoryApiService
    {
        Task<List<RecentSearchDTO>> GetRecentSearchesAsync(CancellationToken ct = default);

        Task RecordSearchAsync(string kind, long entityId, CancellationToken ct = default);

        Task RemoveSearchAsync(long id, CancellationToken ct = default);

        Task ClearSearchesAsync(CancellationToken ct = default);
    }
}
