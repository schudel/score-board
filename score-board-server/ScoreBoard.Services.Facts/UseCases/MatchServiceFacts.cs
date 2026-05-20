using System.Threading.Tasks;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class MatchServiceFacts
    {
        private readonly IMatchService testee;

        public MatchServiceFacts()
        {
            IMatchRepository matchRepository = new MatchRepositoryFake();
            ITeamService teamService = new TeamService(new TeamRepositoryFake());
            testee = new MatchService(matchRepository, 
                new RatingService(new RatingRepositoryFake(),
                    matchRepository,
                    teamService, 
                    new RatingHistoryService(new RatingHistoryRepositoryFake())), 
                teamService);
            testee.Initialize("");
        }

        [Fact]
        public async Task GetByExistingIdFact()
        {
            await Task.Delay(0);
            // TODO implement
        }

        // TODO: implement full service facts
    }
}
