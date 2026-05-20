using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface IMatchService
    {
        void Initialize(string dbConnectionString);
        Task<Match> GetById(string id, bool slim = false);
        Task<ICollection<Match>> GetAll(bool slim = false);
        Task Add(Match match, bool calcQuality, bool updateRanking);
        Task Update(string id, Match match, bool calcQuality, bool updateRanking);
        Task Update(Match match, bool calcQuality, bool updateRanking);
        Task Remove(Match match);
        Task Remove(string id);
        Task<ICollection<Match>> GetMatchesByGameId(string id, bool slim = false);
        Task<ICollection<Match>> GetMatchesByPlayerId(string id, bool slim = false);
        Task<long> Count();
        Task<long> CountByGame(string id);
        Task<long> CountByPlayer(string id);
    }
}
