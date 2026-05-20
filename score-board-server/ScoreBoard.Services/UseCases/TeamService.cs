using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Services.UseCases
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            this.teamRepository = teamRepository;
        }

        public void Initialize(string dbConnectionString)
        {
            teamRepository.Initialize(dbConnectionString);
        }

        public async Task<Team> GetById(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await teamRepository.GetById(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<Team>> GetAll() => await teamRepository.GetAll().ConfigureAwait(false);

        public async Task<Team> GetByExistingTeam(Team team) => await teamRepository.GetByExistingTeam(team).ConfigureAwait(false);

        public async Task SetExistingTeamsIfAvailable(Match match)
        {
            Team tempTeam1 = await teamRepository.GetByExistingTeam(match.Team1).ConfigureAwait(false);
            if (tempTeam1 != null)
            {
                match.Team1 = tempTeam1;
            }
            Team tempTeam2 = await teamRepository.GetByExistingTeam(match.Team2).ConfigureAwait(false);
            if (tempTeam2 != null)
            {
                match.Team2 = tempTeam2;
            }
        }

        public async Task<ICollection<string>> GetNames() => await teamRepository.GetNames().ConfigureAwait(false);
    }
}
