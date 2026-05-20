using System;

namespace ScoreBoard.Domain.Models
{
    public class Game
    {
        public Game()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Genre { get; set; }
        public string Image { get; set; }
        public double Beta { get; set; }
        public double DrawProbability { get; set; }
        public double DynamicsFactor { get; set; }
        public double InitialConservativeRating { get; set; }
        public double InitialMean { get; set; }
        public double InitialStandardDeviation { get; set; }
    }
}
