using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface IChatRepository : IBaseRepository
    {
        Task<Chat> GetById(Guid id);
        Task<ICollection<Chat>> GetAll();
        Task Add(Chat chat);
        Task Update(Chat chat);
        Task Remove(Chat chat);
        Task<long> Count();
        Task<ICollection<Chat>> GetByPlayerId(Guid playerId);
        Task<ICollection<Chat>> GetSpecificEntries(int amount, int skip = 0);
    }
}
