using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Output;
using MAUI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MAUI.ViewModel
{
    // Promena lozinke. Provere na klijentu (isto kao Register): svaka lozinka >= 8 karaktera, nova i ponovljena se
    // poklapaju, nova se razlikuje od trenutne. Trenutnu lozinku i konacna pravila proverava server. Posle uspeha
    // server vraca nove tokene (stari refresh token vise ne vazi) - ovde se samo sacuvaju.
    public partial class ChangePasswordViewModel : ObservableValidator
    {
        private readonly IAuthApiService _authApiService;
        private readonly IAuthService _authService;

        public ChangePasswordViewModel(IAuthApiService authApiService, IAuthService authService)
        {
            _authApiService = authApiService;
            _authService = authService;
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Current password is required.")]
        [MinLength(8, ErrorMessage = "Current password must be at least 8 characters long.")]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters long.")]
        private string newPassword = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Please repeat the new password.")]
        [MinLength(8, ErrorMessage = "Repeated password must be at least 8 characters long.")]
        private string repeatNewPassword = string.Empty;

        // Jedno dugme prikazuje/sakriva sva tri polja.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HidePasswords))]
        [NotifyPropertyChangedFor(nameof(TogglePasswordsText))]
        private bool showPasswords;

        public bool HidePasswords => !ShowPasswords;

        public string TogglePasswordsText => ShowPasswords ? "Hide passwords" : "Show passwords";

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string successMessage = string.Empty;

        [RelayCommand]
        private void TogglePasswords() => ShowPasswords = !ShowPasswords;

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            ValidateAllProperties();

            if (HasErrors)
            {
                ErrorMessage = GetErrors().FirstOrDefault()?.ErrorMessage ?? "Please fix the errors above.";
                return;
            }

            if (NewPassword != RepeatNewPassword)
            {
                ErrorMessage = "New passwords do not match.";
                return;
            }

            if (NewPassword == CurrentPassword)
            {
                ErrorMessage = "New password must be different from the current one.";
                return;
            }

            IsBusy = true;
            try
            {
                var tokens = await _authApiService.ChangePasswordAsync(new ChangePasswordRequestDTO
                {
                    CurrentPassword = CurrentPassword,
                    NewPassword = NewPassword
                });

                await _authService.SaveTokensAsync(tokens.Token, tokens.RefreshToken);

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                RepeatNewPassword = string.Empty;
                ClearErrors();
                SuccessMessage = "Password changed.";

                await Task.Delay(1200);
                await Shell.Current.GoToAsync("..");
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Could not change the password. Please try again.";
                System.Diagnostics.Debug.WriteLine($"ChangePasswordAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
