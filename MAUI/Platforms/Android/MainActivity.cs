using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.Firebase.CloudMessaging;

namespace MAUI
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        // Prosledjuje notifikaciju-tap intent Plugin.Firebase-u (npr. kad je app vec u
        // pozadini/foreground-u i korisnik tapne push) - bez ovoga NotificationTapped event
        // ne bi uopste okinuo u tom slucaju (samo pri hladnom startu).
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        }
    }
}
