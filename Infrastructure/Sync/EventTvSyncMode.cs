namespace Infrastructure.Sync
{
    /// <summary>Koje mecove rucno okidan TV sync obradjuje.</summary>
    public enum EventTvSyncMode
    {
        /// <summary>Sve mecove iz Match tabele.</summary>
        All,

        /// <summary>Samo mecove kojima jos nikad nije proveren TV raspored (TvSyncedAt je null) - nastavak posle prekida.</summary>
        Unsynced,

        /// <summary>Samo mecove sa TV redovima koje je napisala stara verzija (BroadcastTime = trenutak sync-a).</summary>
        Repair
    }
}
