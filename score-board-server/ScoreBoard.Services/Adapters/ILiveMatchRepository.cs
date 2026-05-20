using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface ILiveMatchRepository : IBaseRepository
    {
        Task<LiveMatch> GetById(Guid id);
        Task<ICollection<LiveMatch>> GetAll();
        Task Add(LiveMatch liveMatch);
        Task Update(LiveMatch liveMatch);
        Task Remove(LiveMatch liveMatch);
        Task<long> Count();
        Task<ICollection<LiveMatch>> GetByMatchId(Guid matchId);
        Task<ICollection<LiveMatch>> GetAllDistinct();
    }
}
