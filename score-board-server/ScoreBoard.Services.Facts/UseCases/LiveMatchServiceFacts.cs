using System.Threading.Tasks;
using FakeItEasy;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.Helpers;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class LiveMatchServiceFacts
    {
        private readonly ILiveMatchService testee;

        public LiveMatchServiceFacts()
        {
            IEmailService emailService = A.Fake<IEmailService>();
            testee = new LiveMatchService(new LiveMatchRepositoryFake(), new PlayerRepositoryFake(), emailService);
            testee.Initialize("");
        }

        [Fact]
        public async Task GetByExistingIdFact()
        {
            await Task.Delay(0);
            //string id = "";
            //LiveMatch liveMatch = await testee.GetById(id).ConfigureAwait(false);

            //liveMatch.Id.Should().Be(Guid.Parse(id));
        }

        // TODO: implement full service facts
    }
}
