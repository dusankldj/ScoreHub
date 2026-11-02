using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.DTO.Output;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/friends")]
    [Authorize]
    public class FriendsController : ControllerBase
    {
        private const int MaxPageSize = 50;

        private readonly ScoreHubDbContext _context;

        public FriendsController(ScoreHubDbContext context)
        {
            _context = context;
        }

        // Prihvaceni prijatelji ulogovanog korisnika, POSLEDNJE DODATI PRVI - server paging (skip/take): klijent
        // trazi prvih 5, pa narednih po 10 ("Show 10 more"), umesto da se cela lista ucita odjednom.
        [HttpGet]
        public async Task<ActionResult<FriendsPageDTO>> GetFriends([FromQuery] int skip = 0, [FromQuery] int take = 5, CancellationToken ct = default)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var me = userId.Value;
            skip = Math.Max(skip, 0);
            take = Math.Clamp(take, 1, MaxPageSize);

            // Prijateljstvo je jedan red po paru - korisnik moze biti sa obe strane (zahtev je poslao ili primio).
            var query = _context.Friendships
                .AsNoTracking()
                .Where(f => f.Status == FriendshipStatus.Accepted && (f.RequesterId == me || f.AddresseeId == me));

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(f => f.AcceptedAt)
                .ThenByDescending(f => f.Id)
                .Skip(skip)
                .Take(take)
                .Select(f => new FriendDTO
                {
                    UserId = f.RequesterId == me ? f.AddresseeId : f.RequesterId,
                    Username = f.RequesterId == me ? f.Addressee.Username : f.Requester.Username,
                    FriendsSince = f.AcceptedAt ?? f.CreatedAt
                })
                .ToListAsync(ct);

            return Ok(new FriendsPageDTO { Items = items, Total = total });
        }

        // Dolazni zahtevi za prijateljstvo (najnoviji prvi). Lista je kratka, pa bez paginacije.
        [HttpGet("requests")]
        public async Task<ActionResult<List<FriendRequestDTO>>> GetIncomingRequests(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var requests = await _context.Friendships
                .AsNoTracking()
                .Where(f => f.Status == FriendshipStatus.Pending && f.AddresseeId == userId.Value)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FriendRequestDTO { UserId = f.RequesterId, Username = f.Requester.Username, RequestedAt = f.CreatedAt })
                .ToListAsync(ct);

            return Ok(requests);
        }

        // "Add friend". Ako je ta osoba VEC poslala zahtev meni, ovo ga prihvata (oboje to zele). Idempotentno.
        [HttpPost("{targetUserId:long}/request")]
        public async Task<ActionResult<RelationshipDTO>> SendRequest(long targetUserId, CancellationToken ct)
        {
            var me = GetUserId();
            if (me is null)
                return Unauthorized();

            if (me.Value == targetUserId)
                return BadRequest(new ErrorResponseDTO { Errors = { "You cannot add yourself as a friend." } });

            if (!await _context.Users.AnyAsync(u => u.Id == targetUserId && !u.IsDeleted, ct))
                return NotFound();

            var existing = await _context.FindFriendshipAsync(me.Value, targetUserId, ct);

            if (existing is null)
            {
                _context.Friendships.Add(new Friendship
                {
                    RequesterId = me.Value,
                    AddresseeId = targetUserId,
                    Status = FriendshipStatus.Pending
                });
            }
            else if (existing.Status == FriendshipStatus.Pending && existing.AddresseeId == me.Value)
            {
                existing.Status = FriendshipStatus.Accepted;
                existing.AcceptedAt = DateTime.UtcNow;
            }

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                // Dva istovremena zahteva za isti par (unique indeks) - drugi gubi, stanje se vraca ispod.
            }

            return Ok(new RelationshipDTO { Relationship = await _context.GetRelationshipAsync(me.Value, targetUserId, ct) });
        }

        // Ponistavanje MOG poslatog zahteva ("Request sent" -> "Add friend"). Prihvacen zahtev se ovde ne dira.
        [HttpDelete("{targetUserId:long}/request")]
        public async Task<ActionResult<RelationshipDTO>> CancelRequest(long targetUserId, CancellationToken ct)
        {
            var me = GetUserId();
            if (me is null)
                return Unauthorized();

            var pending = await _context.Friendships.FirstOrDefaultAsync(
                f => f.RequesterId == me.Value && f.AddresseeId == targetUserId && f.Status == FriendshipStatus.Pending, ct);

            if (pending is not null)
            {
                _context.Friendships.Remove(pending);
                await _context.SaveChangesAsync(ct);
            }

            return Ok(new RelationshipDTO { Relationship = await _context.GetRelationshipAsync(me.Value, targetUserId, ct) });
        }

        // Prihvatanje zahteva koji je targetUserId poslao MENI.
        [HttpPost("{targetUserId:long}/accept")]
        public async Task<ActionResult<RelationshipDTO>> AcceptRequest(long targetUserId, CancellationToken ct)
        {
            var me = GetUserId();
            if (me is null)
                return Unauthorized();

            var pending = await _context.Friendships.FirstOrDefaultAsync(
                f => f.RequesterId == targetUserId && f.AddresseeId == me.Value && f.Status == FriendshipStatus.Pending, ct);

            if (pending is not null)
            {
                pending.Status = FriendshipStatus.Accepted;
                pending.AcceptedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }

            return Ok(new RelationshipDTO { Relationship = await _context.GetRelationshipAsync(me.Value, targetUserId, ct) });
        }

        // Odbijanje zahteva koji je targetUserId poslao MENI (red se brise - moze ponovo da posalje).
        [HttpPost("{targetUserId:long}/decline")]
        public async Task<ActionResult<RelationshipDTO>> DeclineRequest(long targetUserId, CancellationToken ct)
        {
            var me = GetUserId();
            if (me is null)
                return Unauthorized();

            var pending = await _context.Friendships.FirstOrDefaultAsync(
                f => f.RequesterId == targetUserId && f.AddresseeId == me.Value && f.Status == FriendshipStatus.Pending, ct);

            if (pending is not null)
            {
                _context.Friendships.Remove(pending);
                await _context.SaveChangesAsync(ct);
            }

            return Ok(new RelationshipDTO { Relationship = await _context.GetRelationshipAsync(me.Value, targetUserId, ct) });
        }

        // Uklanjanje prijatelja (X ikonica na UserPage) - sa bilo koje strane veze.
        [HttpDelete("{targetUserId:long}")]
        public async Task<ActionResult<RelationshipDTO>> RemoveFriend(long targetUserId, CancellationToken ct)
        {
            var me = GetUserId();
            if (me is null)
                return Unauthorized();

            var friendship = await _context.FindFriendshipAsync(me.Value, targetUserId, ct);

            if (friendship is not null && friendship.Status == FriendshipStatus.Accepted)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync(ct);
            }

            return Ok(new RelationshipDTO { Relationship = await _context.GetRelationshipAsync(me.Value, targetUserId, ct) });
        }

        private long? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var id) ? id : null;
        }
    }
}
