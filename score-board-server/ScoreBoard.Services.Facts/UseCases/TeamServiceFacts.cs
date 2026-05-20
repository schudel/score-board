using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class TeamServiceFacts
    {
        private readonly ITeamService testee;
        private readonly FakeData fakeData;

        public TeamServiceFacts()
        {
            fakeData = new FakeData();
            testee = new TeamService(new TeamRepositoryFake());
            testee.Initialize("");
        }

        [Fact]
        public async Task GetByExistingIdFact()
        {
            string id = FakeData.TeamFifa1Id;
            Team team = await testee.GetById(id).ConfigureAwait(false);

            team.Id.Should().Be(Guid.Parse(id));
        }

        [Fact]
        public async Task GetByNoValidIdFact()
        {
            await Task.Delay(0);
            const string id = "NotAValidId";
            Func<Task> f = async () => await testee.GetById(id).ConfigureAwait(false);

            f.Should().Throw<Exception>();
        }

        [Fact]
        public async Task GetByNonExistingIdFact()
        {
            string id = "40F3089B-1219-4AB6-B371-B612BB6CF0C0";
            Team team = await testee.GetById(id).ConfigureAwait(false);

            team.Should().BeNull();
        }

        [Fact]
        public async Task GetAllFact()
        {
            ICollection<Team> teams = await testee.GetAll().ConfigureAwait(false);

            teams.Should().NotBeNull();
            teams.Should().HaveCount(6);
        }

        [Fact]
        public async Task GetByExistingTeamFact()
        {
            Team existingTeam = await testee.GetByExistingTeam(fakeData.FakeTeams[0]).ConfigureAwait(false);

            existingTeam.Should().NotBeNull();
            existingTeam.Name.Should().Be(fakeData.FakeTeams[0].Name);
        }

        [Fact]
        public async Task GetByExistingTeamWithNullFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.GetByExistingTeam(null).ConfigureAwait(false);

            f.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public async Task SetExistingTeamsIfAvailableFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.SetExistingTeamsIfAvailable(fakeData.FakeMatches[0]);
            
            f.Should().NotThrow();
        }


        [Fact]
        public async Task GetNamesFact()
        {
            ICollection<string> names = await testee.GetNames().ConfigureAwait(false);

            IList<string> fakeTeams = new List<string>();
            foreach (string team in FakeData.FifaTeams)
            {
                fakeTeams.Add(team);
            }
            foreach (string team in FakeData.RocketLeagueTeams)
            {
                fakeTeams.Add(team);
            }

            names.Should().BeEquivalentTo(fakeTeams);
        }
    }
}
