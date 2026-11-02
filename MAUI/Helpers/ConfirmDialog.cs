using MAUI.Views;

namespace MAUI.Helpers
{
    // Potvrdni modal "pitanje + objasnjenje + No/Yes" koji se moze pozvati iz bilo kog ViewModel-a
    // (brisanje omiljenog tima, logout...). Vraca true samo za "Yes"; zatvaranje bilo kako drugacije je "No".
    // Otvara se kao providan modal preko trenutne stranice.
    public static class ConfirmDialog
    {
        private static bool _isOpen;

        public static async Task<bool> ShowAsync(string title, string message, string yesText = "Yes", string noText = "No", bool italicMessage = false)
        {
            // Dupli klik na ikonicu ne sme da otvori dva modala.
            if (_isOpen || Shell.Current is null)
                return false;

            _isOpen = true;
            try
            {
                var page = new ConfirmDialogPage(title, message, yesText, noText, italicMessage);
                await Shell.Current.Navigation.PushModalAsync(page, false);
                return await page.Result;
            }
            finally
            {
                _isOpen = false;
            }
        }
    }
}
