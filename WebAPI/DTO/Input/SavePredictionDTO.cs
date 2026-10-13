using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO.Input
{
    public class SavePredictionDTO
    {
        [Range(0, 999)]
        public int PredictedHomeScore { get; set; }

        [Range(0, 999)]
        public int PredictedAwayScore { get; set; }
    }
}
