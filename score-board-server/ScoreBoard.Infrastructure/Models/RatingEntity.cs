using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class RatingEntity
    {
        public virtual Guid Id { get; set; }
        public virtual double ConservativeRating { get; set; }
        public virtual double Mean { get; set; }
        public virtual double StandardDeviation { get; set; }
        public virtual GameEntity Game { get; set; }
        public virtual PlayerEntity Player { get; set; }

        public virtual Rating GetRating(bool slim = false)
        {
            Rating rating = new Rating
            {
                Id = Id,
                ConservativeRating = ConservativeRating,
                Mean = Mean,
                StandardDeviation = StandardDeviation,
                Game = Game.GetGame(slim),
                Player = Player.GetPlayer(slim)
            };
            return rating;
        }

        public static RatingEntity Create(Rating rating)
        {
            if (rating == null)
            {
                return null;
            }
            RatingEntity ratingEntity = new RatingEntity
            {
                Id = rating.Id,
                ConservativeRating = rating.ConservativeRating,
                Mean = rating.Mean,
                StandardDeviation = rating.StandardDeviation,
                Game = GameEntity.Create(rating.Game),
                Player = PlayerEntity.Create(rating.Player)
            };
            return ratingEntity;
        }
    }
}
