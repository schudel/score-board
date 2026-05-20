using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface IRatingService
    {
        void Initialize(string dbConnectionString);
        Task<Rating> GetById(string id, bool slim = false);
        Task<ICollection<Rating>> GetAll(bool slim = false);
        Task Update(string id, Rating rating);
        Task CalcAllRatings(bool saveToDb = false);
        Task<IDictionary<Guid, Rating>> CalcRating(Match match, IDictionary<Guid, Rating> existingRatings = null, bool saveToDb = false);
        Task<double> CalcMatchQuality(Match match, IDictionary<Guid, Rating> existingRatings = null, bool saveToDb = false);
        Task AddDefaultRating(Game game, Player player);
        void SetDefaultGameRatingValues(Game game);
        Task<ICollection<Rating>> GetRatingsByPlayerId(string playerId);
        Task CalcAllMatchQualities();
        Task<ICollection<RatingHistory>> CalcAllRatingHistories(bool saveToDb = false);
    }
}
