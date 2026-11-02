using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Helpers
{
    // Odnos dva korisnika, gledano iz ugla PRVOG (ulogovanog): "Self" / "Friends" / "RequestSent" (ja sam poslao
    // zahtev, ceka) / "RequestReceived" (on je poslao meni) / "None". Friendship je jedan red po paru, pa se
    // trazi u oba smera.
    public static class FriendshipHelper
    {
        public const string Self = "Self";
        public const string Friends = "Friends";
        public const string RequestSent = "RequestSent";
        public const string RequestReceived = "RequestReceived";
        public const string None = "None";

        public static Task<Friendship?> FindFriendshipAsync(this ScoreHubDbContext context, long a, long b, CancellationToken ct)
            => context.Friendships.FirstOrDefaultAsync(
                f => (f.RequesterId == a && f.AddresseeId == b) || (f.RequesterId == b && f.AddresseeId == a), ct);

        public static string RelationshipOf(Friendship? friendship, long me)
        {
            if (friendship is null)
                return None;

            if (friendship.Status == FriendshipStatus.Accepted)
                return Friends;

            return friendship.RequesterId == me ? RequestSent : RequestReceived;
        }

        public static async Task<string> GetRelationshipAsync(this ScoreHubDbContext context, long me, long other, CancellationToken ct)
        {
            if (me == other)
                return Self;

            return RelationshipOf(await context.FindFriendshipAsync(me, other, ct), me);
        }

        public static async Task<bool> AreFriendsAsync(this ScoreHubDbContext context, long me, long other, CancellationToken ct)
            => await context.GetRelationshipAsync(me, other, ct) == Friends;
    }
}
