using MAUI.DTO.Input;
using MAUI.ViewModel;

namespace MAUI.Views;

public partial class ProfilePage : ContentPage
{
	private readonly ProfileViewModel _viewModel;
	public ProfilePage(ProfileViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProfileCommand.ExecuteAsync(null);
    }

    private async void OnPredictionTapped(object? sender, EventArgs e)
    {
        if (sender is Grid { BindingContext: MyPredictionDTO prediction })
            await _viewModel.PredictionSelectedCommand.ExecuteAsync(prediction.EventId);
    }

    private async void OnRemoveFavoriteTeamTapped(object? sender, EventArgs e)
    {
        if (sender is Label { BindingContext: FavoriteTeamDTO team })
            await _viewModel.RemoveFavoriteTeamCommand.ExecuteAsync(team);
    }
}
