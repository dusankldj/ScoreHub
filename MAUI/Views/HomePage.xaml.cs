using MAUI.ViewModel;

namespace MAUI.Views;

public partial class HomePage : ContentPage
{
	private readonly HomeViewModel _viewModel;
	public HomePage(HomeViewModel viewModel)
	{
		InitializeComponent();

		// Sadrzaj ispod trake: gornja margina prati stvarnu visinu trake (bez padajucih menija).
		NavMenuControl.PropertyChanged += (_, e) =>
		{
			if (e.PropertyName == nameof(Partials.NavMenu.BarHeight))
				ContentHost.Margin = new Thickness(0, NavMenuControl.BarHeight, 0, 0);
		};

		_viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

		await _viewModel.LoadSportsCommand.ExecuteAsync(null);
        await _viewModel.LoadMatchesCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopLiveRefresh();
    }
}