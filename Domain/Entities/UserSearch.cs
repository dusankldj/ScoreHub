using System;

namespace Domain.Entities
{
    public enum UserSearchKind
    {
        Player,
        Team,
        League
    }

    /// <summary>
    /// Poslednja pretrazivanja ulogovanog korisnika ("Recent searches" u search polju). Cuva se IZABRANI
    /// rezultat (Kind + EntityId), ne ukucan tekst - ukucani stringovi zbog debounce-a su delimicni
    /// ("bar", "barc") i beskorisni, dok se izabran entitet moze prikazati (logo/ime/sport) i otvoriti
    /// direktno. EntityId nema pravi FK jer pokazuje na jednu od tri tabele (Players/Teams/Leagues) - sync
    /// ih nikad ne brise (samo upsert), pa "siroceta" prakticno ne nastaju; citanje ih svejedno preskace.
    /// </summary>
    public class UserSearch : BaseEntity.BaseEntity
    {
        public long UserId { get; set; }
        public UserSearchKind Kind { get; set; }
        public long EntityId { get; set; }
        public DateTime SearchedAt { get; set; }

        public User User { get; set; }
    }
}
