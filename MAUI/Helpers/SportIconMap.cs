using System.Collections.Generic;

namespace MAUI.Helpers
{
    public static class SportIconMap
    {
        private static readonly Dictionary<string, SportIcon> _map = new()
        {
            ["Soccer"] = new SportIcon(Icons.FootballIcon, IconFontFamily.FontAwesome),
            ["Basketball"] = new SportIcon(Icons.BasketballIcon, IconFontFamily.FontAwesome),
            ["Tennis"] = new SportIcon(Icons.TennisIcon, IconFontFamily.MaterialDesignIcons),
            ["Volleyball"] = new SportIcon(Icons.VolleyballIcon, IconFontFamily.FontAwesome),
            ["Handball"] = new SportIcon(Icons.HandBallIcon, IconFontFamily.MaterialDesignIcons),
            ["American Football"] = new SportIcon(Icons.AmericanFootballIcon, IconFontFamily.FontAwesome),
            ["Ice Hockey"] = new SportIcon(Icons.IceHockeyIcon, IconFontFamily.FontAwesome),
            ["Motorsport"] = new SportIcon(Icons.MotoSportIcon, IconFontFamily.FontAwesome),
            ["Fighting"] = new SportIcon(Icons.FightingIcon, IconFontFamily.FontAwesome),
            ["Cycling"] = new SportIcon(Icons.CyclingIcon, IconFontFamily.FontAwesome),
            ["ESports"] = new SportIcon(Icons.ESportsIcon, IconFontFamily.FontAwesome),
            ["Athletics"] = new SportIcon(Icons.AthleticsIcon, IconFontFamily.FontAwesome),
            ["Shooting"] = new SportIcon(Icons.ShootingIcon, IconFontFamily.FontAwesome),
            ["Watersports"] = new SportIcon(Icons.WatersportsIcon, IconFontFamily.FontAwesome),
        };

        private static readonly SportIcon _fallback = new(Icons.TrophyIcon, IconFontFamily.FontAwesome);

        public static SportIcon GetIcon(string sportName)
            => _map.TryGetValue(sportName, out var icon) ? icon : _fallback;
    }
}
