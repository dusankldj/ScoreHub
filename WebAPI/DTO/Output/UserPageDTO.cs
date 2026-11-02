namespace WebAPI.DTO.Output
{
    public class UserSearchResultDTO
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    // Tudji profil - svima samo korisnicko ime i datum registracije; ime i prezime (FullName) samo prijateljima.
    // Email se nikad ne salje.
    // Relationship vidi FriendshipHelper: Self / Friends / RequestSent / RequestReceived / None.
    public class UserPageDTO
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime MemberSince { get; set; }
        public string Relationship { get; set; } = "None";

        // "Ime Prezime" - SAMO kad su korisnici prijatelji (inace null); server ga ne salje ostalima.
        public string? FullName { get; set; }
    }

    public class RelationshipDTO
    {
        public string Relationship { get; set; } = "None";
    }

    public class FriendRequestDTO
    {
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
    }
}
