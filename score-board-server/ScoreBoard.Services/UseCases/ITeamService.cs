using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface ITeamService
    {
        void Initialize(string dbConnectionString);
        Task<Team> GetById(string id);
        Task<ICollection<Team>> GetAll();
        Task<Team> GetByExistingTeam(Team team);
        Task SetExistingTeamsIfAvailable(Match match);
        Task<ICollection<string>> GetNames();
    }
}
