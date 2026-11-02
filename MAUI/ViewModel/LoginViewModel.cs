using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MAUI.DTO.Output;
using MAUI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MAUI.ViewModel
{
    public partial class LoginViewModel : ObservableValidator
    {
        #region Fields
        private readonly IAuthApiService _authApiService;
        private readonly IAuthService _authService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IDeviceTokenApiService _deviceTokenApiService;
        #endregion

        #region Constructor
        public LoginViewModel(IAuthApiService authApiService, IAuthService authService, IPushNotificationService pushNotificationService, IDeviceTokenApiService deviceTokenApiService)
        {
            _authApiService = authApiService;
            _authService = authService;
            _pushNotificationService = pushNotificationService;
            _deviceTokenApiService = deviceTokenApiService;

            // Posle uspesnog "Forgot password" toka vracamo se ovde sa porukom (ForgotPasswordViewModel salje poruku).
            WeakReferenceMessenger.Default.Register<LoginViewModel, Messages.PasswordResetCompletedMessage>(this, (recipient, _) =>
            {
                recipient.ErrorMessage = string.Empty;
                recipient.InfoMessage = "Password changed. Please log in with your new password.";
            });
        }
        #endregion

        #region Properties
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        [RegularExpression(@"^[a-z0-9@_.]*$", ErrorMessage = "Username can only contain lowercase English letters, digits, and the characters @ _ .")]
        private string username = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string infoMessage = string.Empty;
        #endregion

        #region Methods
        [RelayCommand]
        private async Task LoginAsync()
        {
            InfoMessage = string.Empty;
            ValidateAllProperties();

            if (HasErrors)
            {
                ErrorMessage = GetErrors().FirstOrDefault()?.ErrorMessage ?? "Please fix the errors above.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var result = await _authApiService.LoginAsync(new LoginRequestDTO
                {
                    Username = Username,
                    Password = Password
                });

                await _authService.SaveTokensAsync(result.Token, result.RefreshToken);
                await RegisterDeviceTokenAsync();
                await Shell.Current.GoToAsync("//home");
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Invalid username or password.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        
        private async Task RegisterDeviceTokenAsync()
        {
            try
            {
                await _pushNotificationService.RequestPermissionAsync();
                var token = await _pushNotificationService.GetTokenAsync();

                if (!string.IsNullOrWhiteSpace(token))
                    await _deviceTokenApiService.RegisterTokenAsync(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RegisterDeviceTokenAsync failed: {ex}");
            }
        }
        #endregion
    }
}
