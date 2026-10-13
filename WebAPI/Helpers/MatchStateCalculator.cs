using Domain.Entities;

namespace WebAPI.Helpers
{
    public static class MatchStateCalculator
    {

        private static readonly string[] FinishedStatuses =
        {
            "FT", "AET", "PEN", "AOT", "AP", "AWD", "WO", "CANC", "POST", "PST", "ADB"
        };
        private static readonly TimeSpan LiveWindow = TimeSpan.FromHours(3);

        public static string ComputeState(DateTime eventDate, string? status, DateTime nowUtc)
        {
            if (status == "TBD")
                return "Upcoming";

            if (eventDate > nowUtc)
                return "Upcoming";

            if (status is not null && FinishedStatuses.Contains(status))
                return "Finished";

            if (nowUtc <= eventDate + LiveWindow)
                return "Live";

            return "Finished";
        }

        public static string ComputeState(Match match, DateTime nowUtc)
            => ComputeState(match.EventDate, match.Status, nowUtc);
    }
}
