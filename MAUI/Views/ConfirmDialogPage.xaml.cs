namespace MAUI.Views;

public partial class ConfirmDialogPage : ContentPage
{
    private readonly TaskCompletionSource<bool> _result = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task<bool> Result => _result.Task;

    public ConfirmDialogPage(string title, string message, string yesText, string noText, bool italicMessage = false)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        MessageLabel.Text = message;
        MessageLabel.FontAttributes = italicMessage ? FontAttributes.Italic : FontAttributes.None;
        YesButton.Text = yesText;
        NoButton.Text = noText;
    }

    private async void OnYesClicked(object? sender, EventArgs e) => await CloseAsync(true);

    private async void OnNoClicked(object? sender, EventArgs e) => await CloseAsync(false);

    // Rezultat se saopstava TEK kad se modal zaista ukloni - pozivalac (npr. brisanje tima) nastavlja posle zatvaranja.
    private async Task CloseAsync(bool confirmed)
    {
        try
        {
            await Navigation.PopModalAsync(false);
        }
        finally
        {
            _result.TrySetResult(confirmed);
        }
    }

    // Hardverski "back" = "No" (bez ovoga bi modal bio zatvoren a pozivalac bi ostao da ceka).
    protected override bool OnBackButtonPressed()
    {
        _ = CloseAsync(false);
        return true;
    }
}
