using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class StandingsConfiguration : SyncedEntityConfiguration<Standings>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<Standings> builder)
        {
            builder.Property(x => x.Season).IsRequired();

            builder.HasOne(x => x.Team)
                   .WithMany()
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(x => x.League)
                   .WithMany()
                   .HasForeignKey(x => x.LeagueId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasIndex(x => new { x.LeagueId, x.Season });

            // Logicki kljuc jednog reda tabele - jedan tim ima TACNO jednu poziciju po
            // ligi+sezoni. ExternalId (idStanding) NIJE stabilan - TheSportsDB ga menja pri
            // svakom prera cunu tabele, pa NE moze biti unique kljuc (potvrdjeno u praksi:
            // duplikati do 60 redova za isti tim pre ovog fiksa). Ovo je defense-in-depth na
            // nivou baze - kod (UpsertStandingsAsync) vec trazi postojeci red po ovoj kombinaciji.
            builder.HasIndex(x => new { x.LeagueId, x.TeamId, x.Season }).IsUnique();
        }
    }
}
