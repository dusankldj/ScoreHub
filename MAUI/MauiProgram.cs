using MAUI.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Core.Platforms.Android;

namespace MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FA7Solid");
                    fonts.AddFont("Font Awesome 7 Free-Regular-400.otf", "FA7Regular");
                    fonts.AddFont("Font Awesome 7 Brands-Regular-400.otf", "FA7Brands");
                    fonts.AddFont("materialdesignicons-webfont.ttf", "MDI");
                })
                .AddAppConfiguration()
                .AddAppServices()
                .AddViewModel()
                .AddViews();

            // Firebase
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                    CrossFirebase.Initialize(activity, () => Platform.CurrentActivity)));
            });

        #if DEBUG
    		builder.Logging.AddDebug();
        #endif

            var app = builder.Build();

            try
            {
                using var scope = app.Services.CreateScope();
                _ = scope.ServiceProvider.GetRequiredService<ViewModel.HomeViewModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== DI RESOLUTION FAILED ===");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine("============================");

                var crashBuilder = MauiApp.CreateBuilder();
                crashBuilder.UseMauiApp(_ => new CrashApp(ex));
                return crashBuilder.Build();
            }

            return app;
        }
    }

    public class CrashApp : Application
    {
        private readonly Exception _exception;

        public CrashApp(Exception ex)
        {
            _exception = ex;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var page = new ContentPage
            {
                Content = new ScrollView
                {
                    Content = new Label
                    {
                        Text = _exception.ToString(),
                        Padding = new Thickness(20),
                        LineBreakMode = LineBreakMode.WordWrap,
                        TextColor = Colors.Black
                    }
                },
                BackgroundColor = Colors.White
            };

            return new Window(page);
        }
    }
}
