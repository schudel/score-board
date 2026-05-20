using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Fakes.Infrastructure.Repositories
{
    public class TeamRepositoryFake : ITeamRepository
    {
        private FakeData fakeData;

        public void Initialize(string connectionString)
        {
            fakeData = new FakeData();
        }

        public async Task<Team> GetById(Guid id)
        {
            await Task.Delay(0);
            return fakeData.FakeTeams.SingleOrDefault(t => t.Id == id);
        }

        public async Task<ICollection<Team>> GetAll()
        {
            await Task.Delay(0);
            return fakeData.FakeTeams;
        }

        public async Task<Team> GetByExistingTeam(Team team)
        {
            await Task.Delay(0);
            return fakeData.FakeTeams.SingleOrDefault(t => t.Id == team.Id);
        }

        public async Task<ICollection<string>> GetNames()
        {
            await Task.Delay(0);
            IList<string> fakeTeams = new List<string>();
            foreach (string team in FakeData.FifaTeams)
            {
                fakeTeams.Add(team);
            }
            foreach (string team in FakeData.RocketLeagueTeams)
            {
                fakeTeams.Add(team);
            }
            return fakeTeams;
        }
    }
}
