using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    /// <summary>
    /// lookup/player/{id}. Napomena: ovo je oblik za POJEDINAČNI lookup (imamo ga potvrđenog) -
    /// "otkrivanje" igrača po timu (lista svih igrača jednog tima) ide preko drugog,
    /// još nepotvrđenog endpointa.
    /// </summary>
    public class SportsDbPlayer
    {
        [JsonPropertyName("idPlayer")]
        public string IdPlayer { get; set; } = string.Empty;

        [JsonPropertyName("idTeam")]
        public string? IdTeam { get; set; }

        // Drugi tim (najcesce reprezentacija dok je idTeam klub, ali API ne garantuje redosled) -
        // npr. Nikola Jokic: idTeam=Denver Nuggets, idTeam2=Serbia Basketball.
        [JsonPropertyName("idTeam2")]
        public string? IdTeam2 { get; set; }

        [JsonPropertyName("strPlayer")]
        public string StrPlayer { get; set; } = string.Empty; // puno ime, npr. "Kylian Mbappé"

        [JsonPropertyName("strLastName")]
        public string? StrLastName { get; set; } // API daje prezime posebno - pouzdanije za split nego rucno secenje strPlayer

        [JsonPropertyName("strSport")]
        public string? StrSport { get; set; }

        [JsonPropertyName("dateBorn")]
        public string? DateBorn { get; set; } // "1998-12-20"

        [JsonPropertyName("dateDied")]
        public string? DateDied { get; set; }

        [JsonPropertyName("strBirthLocation")]
        public string? StrBirthLocation { get; set; }

        [JsonPropertyName("strPosition")]
        public string? StrPosition { get; set; }

        [JsonPropertyName("strStatus")]
        public string? StrStatus { get; set; }

        [JsonPropertyName("strDescriptionEN")]
        public string? StrDescriptionEN { get; set; }

        [JsonPropertyName("strHeight")]
        public string? StrHeight { get; set; } // "1.78 m (5 ft 10 in)" - tekst, ne broj

        [JsonPropertyName("strWeight")]
        public string? StrWeight { get; set; } // "163 lb (74 kg)" - tekst, ne broj

        [JsonPropertyName("strWage")]
        public string? StrWage { get; set; }

        [JsonPropertyName("strThumb")]
        public string? StrThumb { get; set; }

        [JsonPropertyName("strFacebook")]
        public string? StrFacebook { get; set; }

        [JsonPropertyName("strInstagram")]
        public string? StrInstagram { get; set; }

        [JsonPropertyName("strTwitter")]
        public string? StrTwitter { get; set; }

        [JsonPropertyName("strYoutube")]
        public string? StrYoutube { get; set; }
    }
}
