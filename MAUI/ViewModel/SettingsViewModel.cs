using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.Helpers;
using MAUI.Services.Interfaces;

namespace MAUI.ViewModel
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IAccountApiService _accountApiService;
        private readonly IAuthService _authService;
        private readonly IUserCacheService _userCache;

        public SettingsViewModel(IAccountApiService accountApiService, IAuthService authService, IUserCacheService userCache)
        {
            _accountApiService = accountApiService;
            _authService = authService;
            _userCache = userCache;
        }

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        // Delete account: potvrda (modal) -> soft delete na serveru -> lokalna odjava -> Home. Server prekida sesije i
        // prijateljstva, a ovde se brisu tokeni i kes profila, da Home odmah izgleda kao za neulogovanog korisnika.
        [RelayCommand]
        private async Task DeleteAccountAsync()
        {
            ErrorMessage = string.Empty;

            if (!await ConfirmDialog.ShowAsync(
                    "Delete your account?",
                    "Your account will be deleted and you will be signed out. You will not be able to log in with it again, and your friends will no longer see you."))
                return;

            IsBusy = true;
            try
            {
                await _accountApiService.DeleteAccountAsync();

                await _authService.LogoutAsync();
                _userCache.Clear();
                await Shell.Current.GoToAsync("//home");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex is InvalidOperationException ? ex.Message : "Could not delete your account. Please try again.";
                System.Diagnostics.Debug.WriteLine($"DeleteAccountAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
