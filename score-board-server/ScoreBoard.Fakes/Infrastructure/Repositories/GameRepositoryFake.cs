using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class GameRepositoryFake : IGameRepository
    {
        private FakeData fakeData;

        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<Game> GetById(Guid id)
        {
            await Task.Delay(0);
            return fakeData.FakeGames.SingleOrDefault(g => g.Id == id);
        }

        public async Task<ICollection<Game>> GetAll()
        {
            await Task.Delay(0);
            return fakeData.FakeGames;
        }

        public async Task Add(Game game)
        {
            await Task.Delay(0);
            fakeData.FakeGames.Add(game);
        }

        public async Task Update(Game game)
        {
            await Task.Delay(0);
            foreach (Game fakeGame in fakeData.FakeGames)
            {
                if (fakeGame.Id == game.Id)
                {
                    fakeGame.Beta = game.Beta;
                    fakeGame.DrawProbability = game.DrawProbability;
                    fakeGame.DynamicsFactor = game.DynamicsFactor; 
                    fakeGame.Genre = game.Genre;
                    fakeGame.Image = game.Image;
                    fakeGame.InitialConservativeRating = game.InitialConservativeRating;
                    fakeGame.InitialMean = game.InitialMean;
                    fakeGame.InitialStandardDeviation = game.InitialStandardDeviation;
                    fakeGame.Name = game.Name;
                    fakeGame.Type = game.Type;
                }
            }
        }

        public async Task Remove(Game game)
        {
            await Task.Delay(0);
            fakeData.FakeGames.Remove(game);
        }

        public async Task<long> Count()
        {
            await Task.Delay(0);
            return fakeData.FakeGames.Count;
        }

        public async Task<ICollection<string>> GetGenres()
        {
            await Task.Delay(0);
            return FakeData.GameGenres;
        }

        public async Task<ICollection<string>> GetTypes()
        {
            await Task.Delay(0);
            return FakeData.GameTypes;
        }
    }
}
