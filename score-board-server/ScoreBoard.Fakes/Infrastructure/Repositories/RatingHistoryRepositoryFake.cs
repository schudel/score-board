using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class RatingHistoryRepositoryFake : IRatingHistoryRepository
    {
        private FakeData fakeData;

        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<RatingHistory> GetById(Guid id, bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakeRatingHistories.SingleOrDefault(r => r.Id == id);
        }

        public async Task<ICollection<RatingHistory>> GetAll(bool slim = false)
        {
            await Task.Delay(0);
            return fakeData.FakeRatingHistories;
        }

        public async Task Add(RatingHistory ratingHistory)
        {
            await Task.Delay(0);
            fakeData.FakeRatingHistories.Add(ratingHistory);
        }

        public async Task Update(RatingHistory ratingHistory)
        {
            await Task.Delay(0);
            foreach (RatingHistory fakeRatingHistory in fakeData.FakeRatingHistories)
            {
                if (fakeRatingHistory.Id == ratingHistory.Id)
                {
                    fakeRatingHistory.ConservativeRating = ratingHistory.ConservativeRating;
                    fakeRatingHistory.DateTime = ratingHistory.DateTime;
                    fakeRatingHistory.GameId = ratingHistory.GameId;
                    fakeRatingHistory.MatchId = ratingHistory.MatchId;
                    fakeRatingHistory.Mean = ratingHistory.Mean;
                    fakeRatingHistory.Player = ratingHistory.Player;
                    fakeRatingHistory.StandardDeviation = ratingHistory.StandardDeviation;
                }
            }
        }

        public async Task Remove(RatingHistory ratingHistory)
        {
            await Task.Delay(0);
            fakeData.FakeRatingHistories.Remove(ratingHistory);
        }
    }
}
