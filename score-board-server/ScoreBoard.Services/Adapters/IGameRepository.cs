using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface IGameRepository : IBaseRepository
    {
        Task<Game> GetById(Guid id);
        Task<ICollection<Game>> GetAll();
        Task Add(Game game);
        Task Update(Game game);
        Task Remove(Game game);
        Task<long> Count();
        Task<ICollection<string>> GetGenres();
        Task<ICollection<string>> GetTypes();
    }
}
