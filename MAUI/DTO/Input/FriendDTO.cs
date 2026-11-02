namespace MAUI.DTO.Input
{
    public class FriendDTO
    {
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime FriendsSince { get; set; }

        // Isto kao ostali datumi sa servera - stize bez Z/offseta, pa se "pecatira" kao Utc pre konverzije.
        public string FriendsSinceDisplay => $"Friends since {DateTime.SpecifyKind(FriendsSince, DateTimeKind.Utc).ToLocalTime():dd/MM/yyyy}";
    }

    // Jedna "stranica" liste prijatelja sa servera; Total = ukupno prijatelja (ne samo ovde vracenih).
    public class FriendsPageDTO
    {
        public List<FriendDTO> Items { get; set; } = new();
        public int Total { get; set; }
    }
}
