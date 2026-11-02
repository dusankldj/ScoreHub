namespace MAUI.Helpers
{
    // Potvrda uklanjanja prijatelja - isti tekst na Profile (X u listi prijatelja) i na UserPage (X ikonica).
    public static class FriendDialogs
    {
        public static Task<bool> ConfirmRemoveAsync(string username)
            => ConfirmDialog.ShowAsync(
                $"Are you sure you want to remove {username} from your friend list?",
                "Once you remove this user, you will not be able to see their favorite teams and predictions until you become friends again.",
                italicMessage: true);
    }
}
