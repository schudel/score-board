using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class RatingServiceFacts
    {
        private readonly IRatingService testee;

        public RatingServiceFacts()
        {
            IRatingRepository ratingRepository = new RatingRepositoryFake();
            IRatingHistoryRepository ratingHistoryRepository = new RatingHistoryRepositoryFake();
            IMatchRepository matchRepository = new MatchRepositoryFake();
            ITeamRepository teamRepository = new TeamRepositoryFake();

            IRatingHistoryService ratingHistoryService = new RatingHistoryService(ratingHistoryRepository);
            ITeamService teamService = new TeamService(teamRepository);

            testee = new RatingService(ratingRepository, matchRepository, teamService, ratingHistoryService);
            testee.Initialize("");
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
            string id = "82A35507-87A8-4145-963D-22D26B74BD0D";
            Rating rating = await testee.GetById(id).ConfigureAwait(false);

            rating.Should().BeNull();
        }

        [Fact]
        public async Task GetAllFact()
        {
            ICollection<Rating> ratings = await testee.GetAll().ConfigureAwait(false);

            ratings.Should().NotBeNull();
            // ratings.Should().HaveCount(35);
        }
    }
}
