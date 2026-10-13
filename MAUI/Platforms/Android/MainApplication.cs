using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.Platforms.Android.Extensions;
using System.Globalization;

namespace MAUI
{
    [Application]
    public class MainApplication : MauiApplication
    {
        // ID kanala u kome ce Android prikazivati push notifikacije (Android 8+/API 26 zahteva
        // kanal - bez njega notifikacija ne bi uopste bila vidljiva na tim verzijama).
        private const string NotificationChannelId = "com.scorehub.app.general";

        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            CreateNotificationChannel();
            FirebaseCloudMessagingImplementation.NotificationBuilderProvider = BuildNotification;
            // DIJAGNOSTIKA - hvata izuzetke koji se dese na POZADINSKOJ niti (async void,
            // ne-sacekan Task) - obican try/catch oko CreateWindow/CreateMauiApp NE hvata ove,
            // jer se dese van tog poziva, na drugoj niti, i runtime ugasi ceo proces.
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                System.Diagnostics.Debug.WriteLine("=== UNHANDLED EXCEPTION (AppDomain) ===");
                System.Diagnostics.Debug.WriteLine(ex?.ToString() ?? "null exception object");
                System.Diagnostics.Debug.WriteLine("=======================================");
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("=== UNOBSERVED TASK EXCEPTION ===");
                System.Diagnostics.Debug.WriteLine(args.Exception.ToString());
                System.Diagnostics.Debug.WriteLine("=================================");
                args.SetObserved();
            };
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            var notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
            var channel = new NotificationChannel(NotificationChannelId, "General", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
            FirebaseCloudMessagingImplementation.ChannelId = NotificationChannelId;
        }

        // Poziva se za SVAKU dolaznu push poruku (data-only - vidi FcmPushNotificationService
        // na backend-u) - tap/navigacija (OnNewIntent/NotificationTapped) i dalje rade automatski,
        // biblioteka samo koristi builder KOJI MI vratimo umesto svog podrazumevanog.
        private static NotificationCompat.Builder BuildNotification(FCMNotification notification)
        {
            var body = notification.Body;

            // Rezultat prognoze: tri reda (rezultat / "You hit!"|"You missed" / prognoza), a
            // poslednji red je zelen (pogodak) ili crven (promasaj). Bez BigPicture stila jer
            // BigText i BigPicture ne mogu istovremeno.
            if (notification.Data.TryGetValue("type", out var kind) && kind == "predictionResult"
                && notification.Data.TryGetValue("scoreLine", out var scoreLine)
                && notification.Data.TryGetValue("verdict", out var verdict)
                && notification.Data.TryGetValue("predictionLine", out var predictionLine))
            {
                var hit = notification.Data.TryGetValue("hit", out var hitRaw) && hitRaw == "true";
                var fullText = $"{scoreLine}\n{verdict}\n{predictionLine}";
                var spannable = new Android.Text.SpannableString(fullText);
                var predictionColor = hit ? Android.Graphics.Color.Rgb(46, 125, 50) : Android.Graphics.Color.Rgb(211, 47, 47);
                spannable.SetSpan(new Android.Text.Style.ForegroundColorSpan(predictionColor),
                    fullText.Length - predictionLine.Length, fullText.Length, Android.Text.SpanTypes.ExclusiveExclusive);

                return new NotificationCompat.Builder(Android.App.Application.Context, FirebaseCloudMessagingImplementation.ChannelId)
                    .SetSmallIcon(Android.Resource.Drawable.SymDefAppIcon)
                    .SetContentTitle(notification.Title)
                    .SetContentText($"{scoreLine} - {verdict}")
                    .SetStyle(new NotificationCompat.BigTextStyle().BigText(spannable))
                    .SetPriority(NotificationCompat.PriorityDefault)
                    .SetAutoCancel(true);
            }

            // Podsetnik (reminder) - JEDINI tip gde tekst zavisi od datuma/vremena. Golovi i
            // krajnji rezultat (goal/finalScore) su vec timezone-nezavisni (samo imena+skor),
            // za njih koristimo server-formatirani notification.Body direktno.
            if (notification.Data.TryGetValue("type", out var type) && type == "reminder"
                && notification.Data.TryGetValue("eventDateUtc", out var eventDateUtcRaw)
                && DateTime.TryParse(eventDateUtcRaw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var eventDateUtc))
            {
                // .ToLocalTime() ovde koristi TRENUTNU vremensku zonu UREDJAJA (Android sistemsko
                // podesavanje) - ispravno gde god se korisnik fizicki trenutno nalazi, za razliku
                // od formatiranja na serveru (koje bi bilo fiksirano na jednu zonu).
                var localDate = eventDateUtc.ToLocalTime();
                var localNow = DateTime.Now;
                var when = localDate.Date == localNow.Date ? "Today"
                         : localDate.Date == localNow.Date.AddDays(1) ? "Tomorrow"
                         : localDate.ToString("dd/MM");

                notification.Data.TryGetValue("homeTeamName", out var home);
                notification.Data.TryGetValue("awayTeamName", out var away);
                body = $"{home} vs {away} - {when} {localDate:HH:mm}";
            }

            return new NotificationCompat.Builder(Android.App.Application.Context, FirebaseCloudMessagingImplementation.ChannelId)
                .SetSmallIcon(Android.Resource.Drawable.SymDefAppIcon)
                .TrySetBigPictureStyle(notification)
                .SetContentTitle(notification.Title)
                .SetContentText(body)
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetAutoCancel(true);
        }
    }
}
