using System;

namespace ScoreBoard.Domain.Models
{
    public class RatingHistory
    {
        public RatingHistory()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public Guid MatchId { get; set; }
        public Guid GameId { get; set; }
        public Player Player { get; set; }
        public double ConservativeRating { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
    }
}
