using DataAccess;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Infrastructure;
using Infrastructure.Email;
using Infrastructure.Integrations;
using Infrastructure.Integrations.SportsDB;
using Infrastructure.Push;
using Infrastructure.Sync;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.JWT;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

namespace WebAPI
{
    public static class Extensions
    {
        

        /// Registruje DbContext i sve što je vezano za pristup bazi.
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ScoreHubDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sql =>
                    sql.CommandTimeout(120))); // podignuto sa default 30s - sync operacije rade velike batch-eve

            // Ovde kasnije idu repository/unit-of-work registracije, npr:
            // services.AddScoped<ITeamRepository, TeamRepository>();

            return services;
        }

        /// Registruje SportsDB integraciju: opcije (ApiKey iz User Secrets, lige iz appsettings),
        /// tipizirani HttpClient i sync servis. Sve živi u Infrastructure projektu.
        public static IServiceCollection AddSportsDbIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SportsOptions>(configuration.GetSection(SportsOptions.SectionName));

            services.AddHttpClient<ISportsDbClient, SportsDbClient>();

            services.AddScoped<ISportsDbSyncService, SportsDbSyncService>();
            services.AddHostedService<SportsDbStartupSyncService>();
            services.AddHostedService<PlayerSyncBackgroundService>();
            services.AddHostedService<PlayerFormerTeamBackgroundService>();
            services.AddHostedService<MatchScheduleBackgroundService>();
            services.AddHostedService<MatchStatisticsTrackerService>();
            services.AddHostedService<PreMatchLineupBackgroundService>();
            services.AddHostedService<EventTvBackgroundService>();
            services.AddHostedService<EventTimelineBackgroundService>();

            return services;
        }

        /// Registruje query/command handlere (npr. MediatR ili ručno pisane servise).
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // services.AddScoped<IPredictionService, PredictionService>();

            return services;
        }

        /// Registruje push notifikacije (Firebase Cloud Messaging) - podsetnici za mecove,
        /// golovi i krajnji rezultat za omiljene timove. Ako service-account JSON (ServiceAccountKeyPath)
        /// jos nije postavljen, FirebaseMessaging ostaje null i FcmPushNotificationService samo
        /// loguje umesto da salje - ostatak app-a normalno radi i bez toga.
        // E-mail (SMTP) + sestocifreni kodovi za potvrdu. Kredencijali (Email:Address / Email:AppPassword / Email:Username) su u User Secrets.
        public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<IVerificationCodeService, VerificationCodeService>();
            return services;
        }

        public static IServiceCollection AddPushNotifications(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FirebaseOptions>(configuration.GetSection(FirebaseOptions.SectionName));

            services.AddSingleton<FirebaseMessaging?>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<FirebaseOptions>>().Value;
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Infrastructure.Push.Firebase");

                if (string.IsNullOrWhiteSpace(options.ServiceAccountKeyPath) || !File.Exists(options.ServiceAccountKeyPath))
                {
                    logger.LogWarning("Firebase nije konfigurisan (nedostaje service-account JSON na putanji '{Path}') - push notifikacije ce biti preskocene dok se ne doda.", options.ServiceAccountKeyPath);
                    return null;
                }

                // Ovaj factory se poziva lenjo, unutar konstruktora SportsDbSyncService-a (kroz
                // MatchStatisticsTrackerService/MatchScheduleBackgroundService) - BEZ ovog try/catch,
                // los/ostecen service-account fajl bi bacio izuzetak VAN bilo kog try/catch-a u tim
                // servisima, sto default BackgroundServiceExceptionBehavior gasi CEO WebAPI proces.
                try
                {
                    var app = FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(options.ServiceAccountKeyPath) });
                    logger.LogInformation("Firebase inicijalizovan uspesno (service-account: '{Path}').", options.ServiceAccountKeyPath);
                    return FirebaseMessaging.GetMessaging(app);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Firebase inicijalizacija nije uspela - push notifikacije ce biti preskocene.");
                    return null;
                }
            });

            services.AddScoped<IPushNotificationService, FcmPushNotificationService>();
            services.AddScoped<IMatchNotificationService, MatchNotificationService>();
            services.AddHostedService<MatchNotificationBackgroundService>();

            return services;
        }

        /// Registruje autentifikaciju/autorizaciju (JWT itd.) kad budeš dodavao naloge.
        public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddScoped<IJwtService, JwtService>();


            var jwtSection = configuration.GetSection(JwtOptions.SectionName);
            var secretKey = jwtSection["SecretKey"];


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });

            return services;
        }
    }
}
