using MAUI.Views;

namespace MAUI.Helpers
{
    public static class ImagePreviewHelper
    {
        public static async Task ShowAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            await Shell.Current.Navigation.PushModalAsync(new ImagePreviewPage(imageUrl));
        }
    }
}
