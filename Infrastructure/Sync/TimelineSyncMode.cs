namespace Infrastructure.Sync
{
    /// <summary>Koje zavrsene mecove rucno okidan sync toka meca obradjuje.</summary>
    public enum TimelineSyncMode
    {
        /// <summary>Sve zavrsene mecove (FT/AET/PEN) iz baze.</summary>
        All,

        /// <summary>Samo mecove kojima jos nikad nije proveren tok (TimelineSyncedAt je null) - nastavak posle prekida.</summary>
        Unsynced
    }
}
