using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Integrations.SportsDB
{
    public class SportsOptions
    {
        public const string SectionName = "SportsDb";
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public List<string> LeagueExternalIds { get; set; } = new();
        public List<string> SportsIds { get; set; } = new();
    }
}
