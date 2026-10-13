using MAUI.ViewModel;

namespace MAUI.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnLoginLinkTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("login");
    }
}
