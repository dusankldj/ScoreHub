using MAUI.Services;
using MAUI.Services.Interfaces;
using MAUI.ViewModel;
using MAUI.Views;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAUI.Helpers
{
    public static class MauiExtensions
    {
        public static MauiAppBuilder AddAppServices(this MauiAppBuilder builder)
        {
            var baseUrl = builder.Configuration["Api:BaseUrl"] ?? throw new InvalidOperationException("Api:BaseUrl not found in appsettings.json");


            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IPushNotificationService, PushNotificationService>();
            builder.Services.AddTransient<RefreshTokenHelper>();

            builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<ISportsApiService, SportsApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<IUserApiService, UserApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<IDeviceTokenApiService, DeviceTokenApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<IMatchesAPIService, MatchesAPIService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<IEventDetailsApiService, EventDetailsApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<ITeamApiService, TeamApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<IPlayerApiService, PlayerApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<ILeagueApiService, LeagueApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<IPredictionApiService, PredictionApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            builder.Services.AddHttpClient<ISearchHistoryApiService, SearchHistoryApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<RefreshTokenHelper>();

            #region Cache services
            builder.Services.AddSingleton<ISportCacheService, SportsCacheService>();

            builder.Services.AddSingleton<IUserCacheService, UserCacheService>();
            #endregion

            return builder;
        }

        public static MauiAppBuilder AddViewModel(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<EventDetailsViewModel>();
            builder.Services.AddTransient<TeamViewModel>();
            builder.Services.AddTransient<PlayerViewModel>();
            builder.Services.AddTransient<LeagueViewModel>();

            return builder;
        }

        public static MauiAppBuilder AddViews(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<EventDetailsPage>();
            builder.Services.AddTransient<TeamPage>();
            builder.Services.AddTransient<PlayerPage>();
            builder.Services.AddTransient<LeaguePage>();

            return builder;
        }


        public static MauiAppBuilder AddAppConfiguration(this MauiAppBuilder builder)
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").
                GetAwaiter().
                GetResult();

            builder.Configuration.AddJsonStream(stream);


            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            return builder;
        }
    }
}
