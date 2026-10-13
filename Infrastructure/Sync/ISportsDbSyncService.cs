using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Sync
{
    public interface ISportsDbSyncService
    {
        Task SyncSportsAsync(CancellationToken ct = default);
        Task SyncLeagueAndTeams(CancellationToken ct = default);
        Task SyncVenuesAsync(CancellationToken ct = default);
        Task SyncPlayersAsync(CancellationToken ct = default);
        Task SyncMatchesAsync(CancellationToken ct = default);
        Task SyncHistoricalMatchesAsync(CancellationToken ct = default);
        Task SyncTeamEquipmentAsync(CancellationToken ct = default);
        Task SyncMatchDetailsAsync(CancellationToken ct = default);
        Task SyncPlayerCareerDataAsync(CancellationToken ct = default);
        Task SyncPlayerStatisticsAsync(CancellationToken ct = default);
        Task SyncPlayerFormerTeamsAsync(CancellationToken ct = default);
        Task SyncPlayerHonoursAsync(CancellationToken ct = default);
        Task SyncPlayerMilestonesAsync(CancellationToken ct = default);
        Task SyncStubPlayerProfilesAsync(CancellationToken ct = default);
        Task SyncLiveMatchStatisticsAsync(CancellationToken ct = default);
        Task SyncUpcomingLineupsAsync(CancellationToken ct = default);
        Task SyncAllAsync(CancellationToken ct = default);
        Task SyncStandingsForLeagueAsync(League league, CancellationToken ct = default);
        Task SyncStandingsAsync(CancellationToken ct = default);
        Task SyncHistoricalStandingsAsync(CancellationToken ct = default);
        Task SyncEventTvAsync(EventTvSyncMode mode = EventTvSyncMode.All, CancellationToken ct = default);
        Task SyncUpcomingEventTvAsync(CancellationToken ct = default);
        Task SyncEventTimelineAsync(TimelineSyncMode mode = TimelineSyncMode.All, bool allSports = false, CancellationToken ct = default);
        Task SyncRecentEventTimelineAsync(CancellationToken ct = default);
    }
}
