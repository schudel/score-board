using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class RatingDto
    {
        public string Id { get; set; }
        public double ConservativeRating { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
        public GameDto Game { get; set; }
        public PlayerDto Player { get; set; }

        public Rating GetRating()
        {
            if (Guid.TryParse(Id, out Guid guid))
            {
                return new Rating
                {
                    Id = guid,
                    ConservativeRating = ConservativeRating,
                    Mean = Mean,
                    StandardDeviation = StandardDeviation,
                    Game = Game.GetGame(),
                    Player = Player.GetPlayer()
                };
            }
            throw new Exception("No valid Id: \"" + Id + "\"");
        }

        public static RatingDto Create(Rating rating)
        {
            if (rating == null)
            {
                return null;
            }
            RatingDto ratingDto = new RatingDto
            {
                Id = rating.Id.ToString(),
                ConservativeRating = rating.ConservativeRating,
                Mean = rating.Mean,
                StandardDeviation = rating.StandardDeviation,
                Game = GameDto.Create(rating.Game),
                Player = PlayerDto.Create(rating.Player)
            };
            return ratingDto;
        }
    }
}
