using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface IRatingRepository : IBaseRepository
    {
        Task<Rating> GetById(Guid id, bool slim = false);
        Task<ICollection<Rating>> GetAll(bool slim = false);
        Task Add(Rating rating);
        Task Update(Rating rating);
        Task Remove(Rating rating);
        Task<ICollection<Rating>> GetByGameId(Guid gameId);
        Task<ICollection<Rating>> GetByPlayerId(Guid playerId);
        Task<Rating> GetByGameIdAndPlayerId(Guid gameId, Guid playerId, bool slim = false);
    }
}
