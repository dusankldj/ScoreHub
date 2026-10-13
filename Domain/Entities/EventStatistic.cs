using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class EventStatistic : SyncedEntity.SyncedEntity
    {
        public long EventId { get; set; }
        public long? ApiId { get; set; }
        public string EventName { get; set; }
        public string Stat { get; set; }
        public double HomeScore { get; set; }
        public double AwayScore { get; set; }


        public Match Event { get; set; }
    }
}
