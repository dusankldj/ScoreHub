using MAUI.Services.Interfaces;
using Plugin.Firebase.CloudMessaging;

namespace MAUI.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        #region Fields
        public event EventHandler<string>? TokenRefreshed;
        #endregion

        #region Constructor
        public PushNotificationService()
        {
            CrossFirebaseCloudMessaging.Current.TokenChanged += (_, e) => TokenRefreshed?.Invoke(this, e.Token);
        }
        #endregion

        #region Methods
        public async Task<bool> RequestPermissionAsync()
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                if (status != PermissionStatus.Granted)
                    return false;

                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RequestPermissionAsync (FCM) failed: {ex}");
                return false;
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTokenAsync (FCM) failed: {ex}");
                return null;
            }
        }
        #endregion
    }
}
