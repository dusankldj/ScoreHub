using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Output;
using MAUI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MAUI.ViewModel
{
    public partial class RegisterViewModel : ObservableValidator
    {
        #region Fields
        private readonly IAuthApiService _authApiService;
        private const string NamePattern = @"^\p{Lu}\p{Ll}*$";
        #endregion

        #region Constructor
        public RegisterViewModel(IAuthApiService authApiService)
        {
            _authApiService = authApiService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(NamePattern, ErrorMessage = "First name must start with an uppercase letter and contain only letters.")]
        private string firstName = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(NamePattern, ErrorMessage = "Last name must start with an uppercase letter and contain only letters.")]
        private string lastName = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        [RegularExpression(@"^[a-z0-9@_.]*$", ErrorMessage = "Username can only contain lowercase English letters, digits, and the characters @ _ .")]
        private string username = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        private string email = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        private string password = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Please repeat your password.")]
        private string repeatPassword = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;
        #endregion

        #region Methods
        [RelayCommand]
        private async Task RegisterAsync()
        {
            ValidateAllProperties();

            if (HasErrors)
            {
                ErrorMessage = GetErrors().FirstOrDefault()?.ErrorMessage ?? "Please fix the errors above.";
                return;
            }

            if (Password != RepeatPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                await _authApiService.RegisterAsync(new RegisterRequestDTO
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Username = Username,
                    Email = Email,
                    Password = Password
                });

                await Shell.Current.GoToAsync("login");
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Registration failed. Username or email may already be in use.";
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
        #endregion
    }
}
