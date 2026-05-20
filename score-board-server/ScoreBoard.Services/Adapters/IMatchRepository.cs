using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface IMatchRepository : IBaseRepository
    {
        Task<Match> GetById(Guid id, bool slim = false);
        Task<ICollection<Match>> GetAll(bool slim = false);
        Task Add(Match match);
        Task Update(Match match);
        Task Remove(Match match);
        Task<ICollection<Match>> GetMatchesByGameId(Guid id, bool slim = false);
        Task<ICollection<Match>> GetMatchesByPlayerId(Guid id, bool slim = false);
        Task<long> Count();
        Task<long> CountByGame(Guid id);
        Task<long> CountByPlayer(Guid id);
    }
}
