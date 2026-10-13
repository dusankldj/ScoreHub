namespace Infrastructure.Integrations.Exceptions
{
    /// Baca se kad poziv ka SportsDB API-ju ne uspe (mrežna greška, neuspešan HTTP status,
    /// ili odgovor koji ne liči na očekivane podatke - npr. {"Message": "..."} umesto liste).
    /// Nosi Url/HttpMethod da bismo mogli da ih upišemo u Logs tabelu i znamo TAČNO
    /// koji poziv je pukao, bez da moramo da nagađamo iz same poruke greške.
    public class SportsDbApiException : Exception
    {
        public string Url { get; }
        public string HttpMethod { get; }

        public SportsDbApiException(string message, string url, string httpMethod, Exception? inner = null)
            : base(message, inner)
        {
            Url = url;
            HttpMethod = httpMethod;
        }
    }
}
