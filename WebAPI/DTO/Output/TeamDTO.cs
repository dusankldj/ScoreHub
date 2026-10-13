namespace WebAPI.DTO.Output
{
    public class TeamDTO
    {
        public string Name { get; set; }
        public string? ShortName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Stadium { get; set; }
        public string? Country { get; set; }
        public string? SportName { get; set; }
        public List<Squad> Squad { get; set; } = new List<Squad>();
        public List<TeamEquipment> Equipment { get; set; } = new List<TeamEquipment>();
        public List<TeamMatches> Matches { get; set; } = new List<TeamMatches>();
    }

    public class Squad
    {
        public long PlayerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public string Position { get; set; }
    }

    public class TeamEquipment
    {
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public string Username { get; set; }
    }

    public class TeamMatches
    {
        public long EventId { get; set; }
        public DateTime EventDate { get; set; }
        public string League { get; set; }
        public string Season { get; set; }
        public string Round { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public string? HomeTeamShortName { get; set; }
        public string? AwayTeamShortName { get; set; }
        public string HomeTeamLogo { get; set; }
        public string AwayTeamLogo { get; set; }
        public string HomeTeamScore { get; set; }
        public string AwayTeamScore { get; set; }

        // Izracunato stanje (Live/Upcoming/Finished) - isto sto i EventDetailsDTO.Status,
        // koristi ga carousel na TeamPage da ispod Round ispise "Live" ili datum/vreme.
        public string? Status { get; set; }
    }
}
