using MAUI.ViewModel;

namespace MAUI.Views;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _viewModel;

	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	private async void OnChangePasswordTapped(object? sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("changepassword");
	}

	private async void OnEditProfileTapped(object? sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("editprofile");
	}

	private async void OnDeleteAccountTapped(object? sender, TappedEventArgs e)
	{
		await _viewModel.DeleteAccountCommand.ExecuteAsync(null);
	}
}
