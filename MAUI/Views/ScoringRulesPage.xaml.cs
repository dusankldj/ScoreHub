using MAUI.ViewModel;

namespace MAUI.Views;

public partial class ScoringRulesPage : ContentPage
{
    private readonly ScoringRulesViewModel _viewModel;

    public ScoringRulesPage(ScoringRulesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
