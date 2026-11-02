using MAUI.Helpers;

namespace MAUI.Services
{
    public enum SearchResultKind
    {
        Player,
        Team,
        League
    }

    public class SearchResultItem
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public SearchResultKind Kind { get; set; }
        public string? ImageUrl { get; set; }
        public string? SportName { get; set; }

        // Samo za stavke iz "Recent searches" - Id reda u UserSearches (za brisanje X ikonicom).
        // Obican rezultat pretrage ga nema, pa X ikonica nije vidljiva.
        public long? RecentId { get; set; }

        public bool IsRecent => RecentId.HasValue;

        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);

        public bool NoImage => !HasImage;

                // U dropdown-u se desno prikazuju DVA reda, centrirano: sport (npr. "Basketball") pa vrsta ("Player" / "Team" / "League").
        public bool HasSportName => !string.IsNullOrWhiteSpace(SportName);

        public string KindLabel => Kind switch
        {
            SearchResultKind.Player => "Player",
            SearchResultKind.Team => "Team",
            SearchResultKind.League => "League",
            _ => string.Empty
        };

        public string IconGlyph => Kind switch
        {
            SearchResultKind.Player => Icons.UserIcon,
            SearchResultKind.Team => Icons.UsersIcon,
            SearchResultKind.League => Icons.TrophyIcon,
            _ => string.Empty
        };
    }
}
