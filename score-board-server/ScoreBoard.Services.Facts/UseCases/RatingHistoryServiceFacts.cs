using System.Threading.Tasks;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class RatingHistoryServiceFacts
    {
        private readonly IRatingHistoryService testee;

        public RatingHistoryServiceFacts()
        {
            testee = new RatingHistoryService(new RatingHistoryRepositoryFake());
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
