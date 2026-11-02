using System;

namespace Domain.Entities
{
    public enum FriendshipStatus
    {
        Pending,
        Accepted
    }

    /// <summary>
    /// Prijateljstvo izmedju dva korisnika. Jedan red po paru (ne dva): Requester salje zahtev, Addressee ga
    /// prihvata (Status = Accepted, AcceptedAt) ili odbija/ponistava (red se brise). Na listi prijatelja
    /// korisnik moze biti sa bilo koje strane. Zahtev (Pending) jos ne cini prijateljstvo.
    /// </summary>
    public class Friendship : BaseEntity.BaseEntity
    {
        public long RequesterId { get; set; }
        public long AddresseeId { get; set; }
        public FriendshipStatus Status { get; set; }

        // Kad je zahtev prihvacen - po tome se sortira "poslednje dodati prijatelji".
        public DateTime? AcceptedAt { get; set; }

        public User Requester { get; set; }
        public User Addressee { get; set; }
    }
}
