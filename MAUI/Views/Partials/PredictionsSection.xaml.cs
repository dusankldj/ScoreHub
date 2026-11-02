using MAUI.DTO.Input;

namespace MAUI.Views.Partials;

public partial class PredictionsSection : ContentView
{
    public PredictionsSection()
    {
        InitializeComponent();
    }

    // Klik na prognozu -> EventDetails za taj mec (isti obrazac kao ostale liste mecova).
    private async void OnPredictionTapped(object? sender, TappedEventArgs e)
    {
        if (sender is BindableObject { BindingContext: MyPredictionDTO prediction })
            await Shell.Current.GoToAsync($"eventdetails?eventId={prediction.EventId}");
    }
}
