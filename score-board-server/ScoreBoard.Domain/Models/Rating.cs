using System;

namespace ScoreBoard.Domain.Models
{
    public class Rating
    {
        public Rating()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public double ConservativeRating { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
        public Game Game { get; set; }
        public Player Player { get; set; }
    }
}
