using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class RatingHistoryEntity
    {
        public virtual Guid Id { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual Guid MatchId { get; set; }
        public virtual Guid GameId { get; set; }
        public virtual PlayerEntity Player { get; set; }
        public virtual double ConservativeRating { get; set; }
        public virtual double Mean { get; set; }
        public virtual double StandardDeviation { get; set; }

        public virtual RatingHistory GetRatingHistory(bool slim = false)
        {
            RatingHistory ratingHistory = new RatingHistory
            {
                Id = Id,
                DateTime = DateTime,
                MatchId = MatchId,
                GameId = GameId,
                Player = Player.GetPlayer(slim),
                ConservativeRating = ConservativeRating,
                Mean = Mean,
                StandardDeviation = StandardDeviation
            };
            return ratingHistory;
        }

        public static RatingHistoryEntity Create(RatingHistory ratingHistory)
        {
            if (ratingHistory == null)
            {
                return null;
            }
            RatingHistoryEntity ratingHistoryEntity = new RatingHistoryEntity
            {
                Id = ratingHistory.Id,
                DateTime = ratingHistory.DateTime,
                MatchId = ratingHistory.MatchId,
                GameId = ratingHistory.GameId,
                Player = PlayerEntity.Create(ratingHistory.Player),
                StandardDeviation = ratingHistory.StandardDeviation,
                ConservativeRating = ratingHistory.ConservativeRating,
                Mean = ratingHistory.Mean
            };
            return ratingHistoryEntity;
        }
    }
}
