namespace MAUI.Views;

public partial class ImagePreviewPage : ContentPage
{
    public ImagePreviewPage(string imageUrl)
    {
        InitializeComponent();
        PreviewImage.Source = imageUrl;
    }

    private async void OnCloseTapped(object? sender, EventArgs e)
        => await Navigation.PopModalAsync();
}
