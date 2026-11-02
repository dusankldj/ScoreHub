namespace MAUI.DTO.Input
{
    // Rezultat pretrage korisnika (padajuca lista "Search user").
    public class UserSearchResultDTO
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    // Odnos ulogovanog korisnika prema drugom (vrednosti sa servera, vidi FriendshipHelper).
    public static class RelationshipState
    {
        public const string Self = "Self";
        public const string Friends = "Friends";
        public const string RequestSent = "RequestSent";
        public const string RequestReceived = "RequestReceived";
        public const string None = "None";
    }

    // Javni deo tudjeg profila: username, member since, odnos prema ulogovanom korisniku.
    public class UserPageDTO
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime MemberSince { get; set; }
        public string Relationship { get; set; } = RelationshipState.None;

        // "Ime Prezime" - server ga salje samo prijateljima (inace null).
        public string? FullName { get; set; }
    }

    public class RelationshipDTO
    {
        public string Relationship { get; set; } = RelationshipState.None;
    }

    // Dolazni zahtev za prijateljstvo (Profile -> Friends -> "Friend requests").
    public class FriendRequestDTO
    {
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
    }
}
