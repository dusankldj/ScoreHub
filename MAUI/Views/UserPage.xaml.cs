using MAUI.DTO.Input;
using MAUI.ViewModel;

namespace MAUI.Views;

public partial class UserPage : ContentPage
{
    private readonly UserViewModel _viewModel;

    public UserPage(UserViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Omiljeni tim prijatelja -> TeamPage.
    private async void OnFavoriteTeamTapped(object? sender, TappedEventArgs e)
    {
        if (sender is BindableObject { BindingContext: FavoriteTeamDTO team })
            await _viewModel.FavoriteTeamSelectedCommand.ExecuteAsync(team.TeamId);
    }
}
