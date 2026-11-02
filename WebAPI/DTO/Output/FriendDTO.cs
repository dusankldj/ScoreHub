namespace WebAPI.DTO.Output
{
    public class FriendDTO
    {
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime FriendsSince { get; set; }
    }

    // Jedna "stranica" liste prijatelja (server paging): Total omogucava klijentu da zna ima li jos.
    public class FriendsPageDTO
    {
        public List<FriendDTO> Items { get; set; } = new();
        public int Total { get; set; }
    }
}
