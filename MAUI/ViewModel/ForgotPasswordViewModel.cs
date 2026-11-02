using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MAUI.DTO.Input;
using MAUI.Messages;
using MAUI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MAUI.ViewModel
{
    // Forgot password, tri koraka na jednoj stranici:
    //  1) e-mail -> sestocifreni kod stize na adresu naloga (server UVEK odgovara isto, da se ne odaje koji nalozi postoje);
    //  2) kod (vazi 10 min, "Resend" posle 60 s) -> server izdaje jednokratni reset token;
    //  3) nova lozinka (2 polja, >= 8 karaktera) -> lozinka se menja, SVE sesije se opozivaju, vracanje na Login (bez auto-prijave).
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly IAuthApiService _authApiService;

        private string _resetToken = string.Empty;
        private CancellationTokenSource? _timerCts;

        public ForgotPasswordViewModel(IAuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        // ---- koraci ----
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmailStep))]
        [NotifyPropertyChangedFor(nameof(IsCodeStep))]
        [NotifyPropertyChangedFor(nameof(IsPasswordStep))]
        private int step = 1;

        public bool IsEmailStep => Step == 1;

        public bool IsCodeStep => Step == 2;

        public bool IsPasswordStep => Step == 3;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string code = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        [ObservableProperty]
        private string repeatPassword = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HidePasswords))]
        [NotifyPropertyChangedFor(nameof(TogglePasswordsText))]
        private bool showPasswords;

        public bool HidePasswords => !ShowPasswords;

        public string TogglePasswordsText => ShowPasswords ? "Hide passwords" : "Show passwords";

        [ObservableProperty]
        private string sentToText = string.Empty;

        [ObservableProperty]
        private string expiresText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanResend))]
        [NotifyPropertyChangedFor(nameof(ResendText))]
        private int resendSeconds;

        public bool CanResend => ResendSeconds <= 0 && !IsBusy;

        public string ResendText => ResendSeconds > 0 ? $"Resend code in {ResendSeconds}s" : "Resend code";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanResend))]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        // Kod je tacno 6 cifara - sve ostalo se odbacuje cim se ukuca.
        partial void OnCodeChanged(string value)
        {
            var cleaned = new string(value.Where(char.IsDigit).Take(6).ToArray());
            if (cleaned != value)
                Code = cleaned;
        }

        [RelayCommand]
        private void TogglePasswords() => ShowPasswords = !ShowPasswords;

        // ---- korak 1 ----
        [RelayCommand]
        private async Task SendCodeAsync()
        {
            ErrorMessage = string.Empty;
            var trimmed = Email.Trim();

            if (!new EmailAddressAttribute().IsValid(trimmed))
            {
                ErrorMessage = "Please enter a valid email address.";
                return;
            }

            IsBusy = true;
            try
            {
                var sent = await _authApiService.ForgotPasswordAsync(trimmed);
                Email = trimmed;
                EnterCodeStep(sent);
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine($"SendCodeAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ---- korak 2 ----
        [RelayCommand]
        private async Task ConfirmCodeAsync()
        {
            ErrorMessage = string.Empty;

            if (Code.Length != 6)
            {
                ErrorMessage = "Enter the 6-digit code from the email.";
                return;
            }

            IsBusy = true;
            try
            {
                _resetToken = await _authApiService.VerifyResetCodeAsync(Email, Code);
                _timerCts?.Cancel();
                ResendSeconds = 0;
                Step = 3;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine($"ConfirmCodeAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ResendCodeAsync()
        {
            if (!CanResend)
                return;

            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                var sent = await _authApiService.ForgotPasswordAsync(Email);
                Code = string.Empty;
                EnterCodeStep(sent);
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine($"ResendCodeAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // "Use a different email" - nazad na korak 1.
        [RelayCommand]
        private void StartOver()
        {
            _timerCts?.Cancel();
            _resetToken = string.Empty;
            Code = string.Empty;
            ResendSeconds = 0;
            ErrorMessage = string.Empty;
            Step = 1;
        }

        // ---- korak 3 ----
        [RelayCommand]
        private async Task ResetPasswordAsync()
        {
            ErrorMessage = string.Empty;

            if (NewPassword.Length < 8)
            {
                ErrorMessage = "New password must be at least 8 characters long.";
                return;
            }

            if (NewPassword != RepeatPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            IsBusy = true;
            try
            {
                await _authApiService.ResetPasswordAsync(Email, _resetToken, NewPassword);

                // Nazad na Login (ova stranica je otvorena sa njega), uz poruku "Password changed" - bez automatske prijave.
                WeakReferenceMessenger.Default.Send(new PasswordResetCompletedMessage());
                await Shell.Current.GoToAsync("..");
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine($"ResetPasswordAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void EnterCodeStep(CodeSentDTO sent)
        {
            Step = 2;
            SentToText = $"If an account exists for {sent.SentTo}, we sent a 6-digit code to it.";
            ExpiresText = $"The code is valid for {sent.ExpiresInSeconds / 60} minutes. Check your spam folder if you do not see it.";
            StartResendCountdown(sent.ResendAfterSeconds);
        }

        // Odbrojavanje do ponovnog slanja (pauza koju diktira server).
        private void StartResendCountdown(int seconds)
        {
            _timerCts?.Cancel();
            var cts = new CancellationTokenSource();
            _timerCts = cts;
            ResendSeconds = seconds;
            _ = RunCountdownAsync(cts.Token);
        }

        private async Task RunCountdownAsync(CancellationToken ct)
        {
            try
            {
                while (ResendSeconds > 0)
                {
                    await Task.Delay(1000, ct);
                    ResendSeconds--;
                }
            }
            catch (OperationCanceledException)
            {
                // presli smo na sledeci korak ili je poslat nov kod
            }
        }
    }
}
