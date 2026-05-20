using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface ITeamRepository : IBaseRepository
    {
        Task<Team> GetById(Guid id);
        Task<ICollection<Team>> GetAll();
        Task<Team> GetByExistingTeam(Team team);
        Task<ICollection<string>> GetNames();
    }
}
