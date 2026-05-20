using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class RatingRepositoryFake : IRatingRepository
    {
        private FakeData fakeData;
        
        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<Rating> GetById(Guid id, bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakeRatings.SingleOrDefault(r => r.Id == id);
        }

        public async Task<ICollection<Rating>> GetAll(bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakeRatings;
        }

        public async Task Add(Rating rating)
        {
            await Task.Delay(0);
            fakeData.FakeRatings.Add(rating);
        }

        public async Task Update(Rating rating)
        {
            await Task.Delay(0);
            foreach (Rating fakeRating in fakeData.FakeRatings)
            {
                if (fakeRating.Id == rating.Id)
                {
                    fakeRating.Game = rating.Game;
                    fakeRating.ConservativeRating = rating.ConservativeRating;
                    fakeRating.Mean = rating.Mean;
                    fakeRating.StandardDeviation = rating.StandardDeviation;
                    fakeRating.Player = rating.Player;
                }
            }
        }

        public async Task Remove(Rating rating)
        {
            await Task.Delay(0);
            fakeData.FakeRatings.Remove(rating);
        }

        public async Task<ICollection<Rating>> GetByGameId(Guid gameId)
        {
            await Task.Delay(0);
            return fakeData.FakeRatings.Where(r => r.Game.Id == gameId).ToList();
        }

        public async Task<ICollection<Rating>> GetByPlayerId(Guid playerId)
        {
            await Task.Delay(0);
            return fakeData.FakeRatings.Where(r => r.Player.Id == playerId).ToList();
        }

        public async Task<Rating> GetByGameIdAndPlayerId(Guid gameId, Guid playerId, bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakeRatings.SingleOrDefault(r => r.Game.Id == gameId && r.Player.Id == playerId);
        }
    }
}