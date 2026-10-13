using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Prediction : BaseEntity.BaseEntity
    {
        public long UserId { get; set; }
        public long MatchId { get; set; }
        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }
        public int? Points { get; set; }
        public bool Hit { get; set; } = false;


        public User User { get; set; }
        public Match Match { get; set; }
    }
}
