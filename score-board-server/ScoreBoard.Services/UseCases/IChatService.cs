using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface IChatService
    {
        void Initialize(string dbConnectionString);
        Task<Chat> GetById(string id);
        Task<ICollection<Chat>> GetAll();
        Task Add(Chat chat);
        Task Update(string id, Chat chat);
        Task Remove(Chat chat);
        Task Remove(string id);
        Task<long> Count();
        Task<ICollection<Chat>> GetByPlayerId(string playerId);
        Task<ICollection<Chat>> GetSpecificEntries(int amount, int skip = 0);
    }
}
