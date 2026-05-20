using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Infrastructure.Repositories;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Init
{
    public class TestDataFacts
    {
        private const int FakeUserAmount = 8;
        private const int FakeGameAmount = 2;
        private const int FakeMatchAmount = 100;

        private readonly IGameService gameService;
        private readonly IPlayerRepository playerRepository;
        private readonly IMatchService matchService;
        private readonly ITeamService teamService;

        public TestDataFacts()
        {
            IGameRepository gameRepository = new GameRepository();
            gameRepository.Initialize(Constants.ConnectionString);
            playerRepository = new PlayerRepository();
            playerRepository.Initialize(Constants.ConnectionString);
            IMatchRepository matchRepository = new MatchRepository();
            matchRepository.Initialize(Constants.ConnectionString);
            ITeamRepository teamRepository = new TeamRepository();
            teamRepository.Initialize(Constants.ConnectionString);
            IRatingRepository ratingRepository = new RatingRepository();
            ratingRepository.Initialize(Constants.ConnectionString);
            IRatingHistoryRepository ratingHistoryRepository = new RatingHistoryRepository();
            ratingHistoryRepository.Initialize(Constants.ConnectionString);
            IRatingHistoryService ratingHistoryService = new RatingHistoryService(ratingHistoryRepository);
            ratingHistoryService.Initialize(Constants.ConnectionString);

            gameService = new GameService(gameRepository);
            gameService.Initialize(Constants.ConnectionString);
            ISettingsService settingsService = new SettingsService(playerRepository);
            settingsService.Initialize(Constants.ConnectionString);
            teamService = new TeamService(teamRepository);
            teamService.Initialize(Constants.ConnectionString);
            IRatingService ratingService = new RatingService(ratingRepository, matchRepository, teamService, ratingHistoryService);
            ratingService.Initialize(Constants.ConnectionString);
            matchService = new MatchService(matchRepository, ratingService, teamService);
            matchService.Initialize(Constants.ConnectionString);
        }

        [Fact(Skip = "Only for Test Data generation")]
        public async Task TestAddFakeData()
        {
            ICollection<Player> fakePlayers = CreateFakeUsers();
            fakePlayers.Add(TestData.CreateAdminPlayer());
            fakePlayers.Add(TestData.CreateUserPlayer1());
            ICollection<Game> fakeGames = CreateFakeGames();
            fakeGames.Add(TestData.CreateGameFifa());
            fakeGames.Add(TestData.CreateGameRocketLeague());
            fakeGames.Add(TestData.CreateGameToeggelikaste());
            IEnumerable<Match> fakeMatches = CreateFakeMatches(fakePlayers, fakeGames);

            foreach (Player fakePlayer in fakePlayers)
            {
                await playerRepository.Add(fakePlayer).ConfigureAwait(false);
            }
            foreach (Game fakeGame in fakeGames)
            {
                await gameService.Add(fakeGame).ConfigureAwait(false);
            }
            foreach (Match fakeMatch in fakeMatches)
            {
                await teamService.SetExistingTeamsIfAvailable(fakeMatch).ConfigureAwait(false);
                await matchService.Add(fakeMatch, true, true).ConfigureAwait(false);
            }
        }

        private static ICollection<Player> CreateFakeUsers()
        {
            ICollection<Player> fakePlayers = new List<Player>();
            for (int i = 0; i < FakeUserAmount; i++)
            {
                fakePlayers.Add(TestData.CreateTestPlayer());
            }
            return fakePlayers;
        }

        private static ICollection<Game> CreateFakeGames()
        {
            ICollection<Game> fakeGames = new List<Game>();
            for (int i = 0; i < FakeGameAmount; i++)
            {
                fakeGames.Add(TestData.CreateTestGame());
            }
            return fakeGames;
        }

        private static IEnumerable<Match> CreateFakeMatches(ICollection<Player> fakePlayers, ICollection<Game> fakeGames)
        {
            ICollection<Match> fakeMatches = new List<Match>();
            for (int i = 0; i < FakeMatchAmount; i++)
            {
                fakeMatches.Add(TestData.CreateTestMatch(fakePlayers, fakeGames));
            }
            return fakeMatches;
        }
    }
}
