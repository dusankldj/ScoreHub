using MAUI.ViewModel;

namespace MAUI.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnRegisterLinkTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("register");
    }
}
