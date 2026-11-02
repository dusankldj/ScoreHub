using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class ScoreHubDbContext : DbContext
    {
        public ScoreHubDbContext(DbContextOptions<ScoreHubDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScoreHubDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FavoriteTeam> FavoriteTeams { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamEquipment> TeamEquipment { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<EventStatistic> EventStatistics { get; set; }
        public DbSet<EventLineup> EventLineups { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<PlayerFormerTeam> PlayerFormerTeams { get; set; }
        public DbSet<PlayerHonour> PlayerHonours { get; set; }
        public DbSet<PlayerMilestone> PlayerMilestones { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Standings> Standings { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<EventTv> EventTvs { get; set; }
        public DbSet<EventTimeline> EventTimelines { get; set; }

        public DbSet<Log> Logs { get; set; }
        public DbSet<Jwt> Jwt { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }
        public DbSet<UserSearch> UserSearches { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<SportScoringRule> SportScoringRules { get; set; }
        public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    }
}
