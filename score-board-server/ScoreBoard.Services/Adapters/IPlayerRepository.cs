using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface IPlayerRepository : IBaseRepository
    {
        Task<Player> GetById(Guid id, bool slim = false);
        Task<ICollection<Player>> GetAll();
        Task Add(Player player);
        Task Update(Player player);
        Task Remove(Player player);
        Task<Player> GetPlayerByPlayerName(string playerName);
        Task<long> Count();
        Task<Player> GetByEmail(string email);
    }
}
