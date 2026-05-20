using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class RatingHistoryDto
    {
        public string Id { get; set; }
        public DateTime DateTime { get; set; }
        public string MatchId { get; set; }
        public string GameId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public double ConservativeRating { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }

        public static RatingHistoryDto Create(RatingHistory ratingHistory)
        {
            if (ratingHistory == null)
            {
                return null;
            }
            RatingHistoryDto ratingDetailDto = new RatingHistoryDto
            {
                Id = ratingHistory.Id.ToString(),
                DateTime = ratingHistory.DateTime,
                MatchId = ratingHistory.MatchId.ToString(),
                GameId = ratingHistory.GameId.ToString(),
                PlayerId = ratingHistory.Player.Id.ToString(),
                PlayerName = ratingHistory.Player.PlayerName,
                ConservativeRating = ratingHistory.ConservativeRating,
                Mean = ratingHistory.Mean,
                StandardDeviation = ratingHistory.StandardDeviation
            };
            return ratingDetailDto;
        }
    }
}
