using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface IGameService
    {
        void Initialize(string dbConnectionString);
        Task<Game> GetById(string id);
        Task<ICollection<Game>> GetAll();
        Task Add(Game game);
        Task Update(string id, Game game);
        Task Remove(Game game);
        Task Remove(string id);
        Task<long> Count();
        Task<ICollection<string>> GetGenres();
        Task<ICollection<string>> GetTypes();
    }
}
