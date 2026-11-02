using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MAUI.ViewModel
{
    // Edit profile: korisnicko ime + e-mail (forma je popunjena trenutnim podacima).
    //  - Korisnicko ime se menja odmah (server proverava da li je slobodno).
    //  - E-mail se menja u dva koraka: (1) nova adresa + trenutna lozinka -> sestocifreni kod stize na NOVU adresu;
    //    (2) unos koda (vazi 10 min, "Resend" posle pauze od 60 s) -> adresa se menja, a na STARU stize obavestenje.
    public partial class EditProfileViewModel : ObservableObject
    {
        private static readonly Regex UsernamePattern = new(@"^[a-z0-9@_.]{3,}$", RegexOptions.Compiled);

        private readonly IUserApiService _userApiService;
        private readonly IAccountApiService _accountApiService;
        private readonly IUserCacheService _userCache;

        private string _originalUsername = string.Empty;
        private string _originalEmail = string.Empty;
        private CancellationTokenSource? _timerCts;

        public EditProfileViewModel(IUserApiService userApiService, IAccountApiService accountApiService, IUserCacheService userCache)
        {
            _userApiService = userApiService;
            _accountApiService = accountApiService;
            _userCache = userCache;
        }

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NeedsPassword))]
        private string email = string.Empty;

        // Trenutna lozinka treba SAMO kad se menja e-mail (ponovna provera identiteta).
        [ObservableProperty]
        private string currentPassword = string.Empty;

        public bool NeedsPassword => !string.Equals(Email.Trim(), _originalEmail, StringComparison.OrdinalIgnoreCase);

        // ---- korak 2: unos koda ----
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormStep))]
        private bool isCodeStep;

        public bool IsFormStep => !IsCodeStep;

        [ObservableProperty]
        private string code = string.Empty;

        [ObservableProperty]
        private string sentToText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanResend))]
        [NotifyPropertyChangedFor(nameof(ResendText))]
        private int resendSeconds;

        [ObservableProperty]
        private string expiresText = string.Empty;

        public bool CanResend => ResendSeconds <= 0 && !IsBusy;

        public string ResendText => ResendSeconds > 0 ? $"Resend code in {ResendSeconds}s" : "Resend code";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanResend))]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string successMessage = string.Empty;

        // Kod je tacno 6 cifara - sve ostalo se odbacuje cim se ukuca.
        partial void OnCodeChanged(string value)
        {
            var cleaned = new string(value.Where(char.IsDigit).Take(6).ToArray());
            if (cleaned != value)
                Code = cleaned;
        }

        // Poziva stranica pri otvaranju: ucitava SVEZE podatke (ne iz kesa - e-mail se tamo ne pamti).
        public async Task LoadAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var me = await _userApiService.GetMeAsync();
                _originalUsername = me.Username;
                _originalEmail = me.Email;
                Username = me.Username;
                Email = me.Email;
                CurrentPassword = string.Empty;
                OnPropertyChanged(nameof(NeedsPassword));
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load your profile.";
                System.Diagnostics.Debug.WriteLine($"EditProfileViewModel.LoadAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            var newUsername = Username.Trim();
            var newEmail = Email.Trim();
            var usernameChanged = newUsername != _originalUsername;
            var emailChanged = NeedsPassword;

            if (!usernameChanged && !emailChanged)
            {
                ErrorMessage = "Nothing to save - you have not changed anything.";
                return;
            }

            // Provere na klijentu (konacna pravila proverava server).
            if (usernameChanged && !UsernamePattern.IsMatch(newUsername))
            {
                ErrorMessage = "Username must be at least 3 characters and can only contain lowercase English letters, digits, and the characters @ _ .";
                return;
            }

            if (emailChanged)
            {
                if (!new EmailAddressAttribute().IsValid(newEmail))
                {
                    ErrorMessage = "Please enter a valid email address.";
                    return;
                }

                if (CurrentPassword.Length < 8)
                {
                    ErrorMessage = "Enter your current password (at least 8 characters) to change your email.";
                    return;
                }
            }

            IsBusy = true;
            try
            {
                if (usernameChanged)
                {
                    _originalUsername = await _accountApiService.UpdateUsernameAsync(newUsername);
                    Username = _originalUsername;
                    _userCache.Clear();
                    SuccessMessage = "Username updated.";
                }

                if (emailChanged)
                {
                    var sent = await _accountApiService.RequestEmailChangeAsync(newEmail, CurrentPassword);
                    CurrentPassword = string.Empty;
                    EnterCodeStep(sent);
                }
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                SuccessMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine($"SaveAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

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
                _originalEmail = await _accountApiService.ConfirmEmailChangeAsync(Code);
                Email = _originalEmail;
                _userCache.Clear();

                LeaveCodeStep();
                SuccessMessage = "Email address updated.";
                OnPropertyChanged(nameof(NeedsPassword));
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
                var sent = await _accountApiService.ResendEmailCodeAsync();
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

        // Nazad na formu (nezavrsen zahtev na serveru ostaje, ali bez uticaja - kod istice).
        [RelayCommand]
        private void CancelCode()
        {
            LeaveCodeStep();
            Email = _originalEmail;
            ErrorMessage = string.Empty;
        }

        private void EnterCodeStep(DTO.Input.CodeSentDTO sent)
        {
            IsCodeStep = true;
            SentToText = $"We sent a 6-digit code to {sent.SentTo}.";
            ExpiresText = $"The code is valid for {sent.ExpiresInSeconds / 60} minutes. Check your spam folder if you do not see it.";
            StartResendCountdown(sent.ResendAfterSeconds);
        }

        private void LeaveCodeStep()
        {
            _timerCts?.Cancel();
            IsCodeStep = false;
            Code = string.Empty;
            ResendSeconds = 0;
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
                // izasli smo sa koraka ili je poslat nov kod
            }
        }
    }
}
