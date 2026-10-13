using Microsoft.Extensions.DependencyInjection;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;

namespace MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            UserAppTheme = AppTheme.Light;
        }

        // Tap na push notifikaciju (mec-podsetnik/gol/krajnji rezultat - vidi MatchNotificationService
        // na backend-u, svi salju "eventId" u data payload-u) - odvede direktno na taj mec.
        // NAPOMENA: pokriva slucaj kad je app vec pokrenut (u pozadini/foreground-u); "hladan"
        // start iz notifikacije (app potpuno ugasen) bi trazio dodatnu obradu launch Intent-a
        // u MainActivity.OnCreate - ostavljeno za kasnije ako se pokaze da je stvarno potrebno.
        private async void OnNotificationTapped(object? sender, FCMNotificationTappedEventArgs e)
        {
            try
            {
                if (Shell.Current is null)
                    return;

                if (e.Notification.Data.TryGetValue("eventId", out var eventId) && !string.IsNullOrWhiteSpace(eventId))
                    await Shell.Current.GoToAsync($"eventdetails?eventId={eventId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnNotificationTapped failed: {ex}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            try
            {
                // Ovde (ne u konstruktoru App-a) jer CrossFirebase.Initialize (MauiProgram Android
                // OnCreate lifecycle event) mora prvo da odradi svoj posao - CreateWindow se poziva
                // POSLE platform OnCreate-a, konstruktor App-a garancije nema.
                try
                {
                    CrossFirebaseCloudMessaging.Current.NotificationTapped += OnNotificationTapped;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Subscribing to NotificationTapped failed: {ex}");
                }

                return new Window(new AppShell());
            }
            catch (Exception ex)
            {
                var page = new ContentPage
                {
                    Content = new ScrollView
                    {
                        Content = new Label
                        {
                            Text = ex.ToString(),
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
}
