namespace MAUI.Views.Partials;

public partial class TimelineCell : ContentView
{
    // Podrazumevano tamno (za kratki Timeline, na SVETLOJ pozadini stranice) - "Full timeline"
    // kartica ima TAMNU pozadinu (isto kao Lineup/Statistics/Broadcasts) i eksplicitno postavlja
    // White, inace bi tekst bio nevidljiv (tamno na tamnom).
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(TimelineCell), Color.FromArgb("#1A1A2E"));

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public TimelineCell()
    {
        InitializeComponent();
    }
}
