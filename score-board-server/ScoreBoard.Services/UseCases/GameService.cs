using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Services.UseCases
{
    public class GameService : IGameService
    {
        private readonly IGameRepository gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        public void Initialize(string dbConnectionString)
        {
            gameRepository.Initialize(dbConnectionString);
        }

        public async Task<Game> GetById(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await gameRepository.GetById(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<Game>> GetAll() => await gameRepository.GetAll().ConfigureAwait(false);

        public async Task Add(Game game)
        {
            if (game == null)
            {
                throw new Exception("Game is required.");
            }
            await gameRepository.Add(game).ConfigureAwait(false);
        }

        public async Task Update(string id, Game game)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Game g = await gameRepository.GetById(guid).ConfigureAwait(false);
            if (g == null)
            {
                throw new Exception("Game not found");
            }
            await gameRepository.Update(game).ConfigureAwait(false);
        }

        public async Task Remove(Game game)
        {
            if (game == null)
            {
                throw new Exception("Game is required.");
            }
            await gameRepository.Remove(game).ConfigureAwait(false);
        }

        public async Task Remove(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Game game = await gameRepository.GetById(guid).ConfigureAwait(false);
            await gameRepository.Remove(game).ConfigureAwait(false);
        }

        public async Task<long> Count() => await gameRepository.Count().ConfigureAwait(false);

        public async Task<ICollection<string>> GetGenres() => await gameRepository.GetGenres().ConfigureAwait(false);

        public async Task<ICollection<string>> GetTypes() => await gameRepository.GetTypes().ConfigureAwait(false);
    }
}
