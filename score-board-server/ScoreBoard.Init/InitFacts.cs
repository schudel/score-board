using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Infrastructure.Repositories;
using ScoreBoard.Services.Adapters;
using Xunit;

namespace ScoreBoard.Init
{
    public class InitFacts
    {
        private readonly IPlayerRepository playerRepository;
        private readonly IGameRepository gameRepository;

        public InitFacts()
        {
            playerRepository = new PlayerRepository();
            playerRepository.Initialize(Constants.ConnectionString);
            gameRepository = new GameRepository();
            gameRepository.Initialize(Constants.ConnectionString);
        }

        [Fact(Skip="Only for Initialization")]
        public async Task A_TestAddUsersFact()
        {
            Player admin = TestData.CreateAdminPlayer();
            await playerRepository.Add(admin).ConfigureAwait(false);

            //Player player1 = TestData.CreateUserPlayer1();
            //await playerRepository.Add(player1).ConfigureAwait(false);
        }

        [Fact(Skip = "Only for Initialization")]
        public async Task B_TestAddGamesFact()
        {
            Game fifa19 = TestData.CreateGameFifa();
            Game rocketLeague = TestData.CreateGameRocketLeague();
            Game toeggeliKaste = TestData.CreateGameToeggelikaste();

            await gameRepository.Add(fifa19).ConfigureAwait(false);
            await gameRepository.Add(rocketLeague).ConfigureAwait(false);
            await gameRepository.Add(toeggeliKaste).ConfigureAwait(false);
        }
    }
}
