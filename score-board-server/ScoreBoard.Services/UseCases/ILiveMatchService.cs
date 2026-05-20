using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface ILiveMatchService
    {
        void Initialize(string dbConnectionString);
        Task<LiveMatch> GetById(string id);
        Task<ICollection<LiveMatch>> GetAll();
        Task Add(LiveMatch liveMatch);
        Task Update(string id, LiveMatch liveMatch);
        Task Update(LiveMatch liveMatch);
        Task Remove(LiveMatch liveMatch);
        Task Remove(string id);
        Task<long> Count();
        Task<ICollection<LiveMatch>> GetByMatchId(string matchId);
        Task<ICollection<LiveMatch>> GetAllDistinct();
        Task<bool> InvitePlayer(Invitation invitation);
    }
}
