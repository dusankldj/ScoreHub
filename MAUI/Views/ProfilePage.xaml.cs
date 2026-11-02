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

    private async void OnFavoriteTeamTapped(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: FavoriteTeamDTO team })
            await _viewModel.FavoriteTeamSelectedCommand.ExecuteAsync(team.TeamId);
    }

    private async void OnRemoveFavoriteTeamTapped(object? sender, EventArgs e)
    {
        if (sender is Label { BindingContext: FavoriteTeamDTO team })
            await _viewModel.RemoveFavoriteTeamCommand.ExecuteAsync(team);
    }

    // Rezultat pretrage korisnika -> UserPage.
    private async void OnUserSearchResultTapped(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: UserSearchResultDTO user })
            await _viewModel.UserSelectedCommand.ExecuteAsync(user.Id);
    }

    // Red u rang-listi -> UserPage tog prijatelja (sopstveni red se preskace).
    private async void OnLeaderboardRowTapped(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: LeaderboardRowDTO { IsMe: false } row })
            await _viewModel.FriendSelectedCommand.ExecuteAsync(row.UserId);
    }

    // Prijatelj iz liste -> UserPage.
    private async void OnFriendTapped(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: FriendDTO friend })
            await _viewModel.FriendSelectedCommand.ExecuteAsync(friend.UserId);
    }

    // X pored prijatelja -> modal sa potvrdom, pa uklanjanje.
    private async void OnRemoveFriendTapped(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: FriendDTO friend })
            await _viewModel.RemoveFriendCommand.ExecuteAsync(friend);
    }

    // Dolazni zahtev: ime -> UserPage, Accept / Decline -> odgovarajuca komanda.
    private async void OnRequestUserTapped(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: FriendRequestDTO request })
            await _viewModel.FriendSelectedCommand.ExecuteAsync(request.UserId);
    }

    private async void OnAcceptRequestClicked(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: FriendRequestDTO request })
            await _viewModel.AcceptRequestCommand.ExecuteAsync(request.UserId);
    }

    private async void OnDeclineRequestClicked(object? sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: FriendRequestDTO request })
            await _viewModel.DeclineRequestCommand.ExecuteAsync(request.UserId);
    }
}
