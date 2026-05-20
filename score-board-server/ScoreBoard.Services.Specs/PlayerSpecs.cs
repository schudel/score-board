using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.Helpers;
using ScoreBoard.Services.UseCases;
using Xbehave;

namespace ScoreBoard.Services.Specs
{
    public class PlayerSpecs
    {
        private IPlayerService playerService;
        private IRatingService ratingService;
        private ITeamService teamService;
        private IRatingHistoryService ratingHistoryService;
        private PlayerRepositoryFake playerRepositoryFake;
        private IEmailService emailServiceFake;
        private FakeData fakeData;

        [Background]
        // ReSharper disable once UnusedMember.Global
        public void Background()
        {
            "Create FakeData".x(
                () => fakeData = new FakeData());

            "Create Player Repository".x(
                () => playerRepositoryFake = new PlayerRepositoryFake());

            "Create Team Service".x(
                () => teamService = new TeamService(new TeamRepositoryFake()));

            "Create RatingHistory Service".x(
                () => ratingHistoryService = new RatingHistoryService(new RatingHistoryRepositoryFake()));

            "Create Rating Service".x(
                () => ratingService = new RatingService(new RatingRepositoryFake(), 
                    new MatchRepositoryFake(),
                    teamService,
                    ratingHistoryService));

            "Create Email Fake Service".x(
                () => emailServiceFake = A.Fake<IEmailService>());

            "Create Player Service".x(
                () => playerService = new PlayerService(playerRepositoryFake, 
                    new PasswordService(),
                    emailServiceFake, 
                    new SettingsService(playerRepositoryFake), 
                    ratingService));

            "Initialize Team Service".x(
                () => teamService.Initialize(""));

            "Initialize RatingHistory Service".x(
                () => ratingHistoryService.Initialize(""));

            "Initialize Rating Service".x(
                () => ratingService.Initialize(""));

            "Initialize Player Service".x(
                () => playerService.Initialize(""));
        }

        [Scenario]
        public void RegisterNewPlayerAndActivateAccountAndLoginFirstTime(Player player, Func<Task> authenticationFunc, Player authPlayer)
        {
            const string idString = "A80F84C3-A364-4AAB-AB80-40279B565247";
            Guid id = Guid.Parse(idString);
            const string playerName = "PlayerXYZ";
            const string password = "SecurePassword123";
            const string email = "benjamin@schudel.rocks";
            const string firstName = "Hans";
            const string lastName = "Muster";

            "Given a new Player".x(
                () => player = new Player { Id = id, PlayerName = playerName, Email = email, FirstName = firstName, LastName = lastName });

            "When Register the new Player".x(
                async () => await playerService.Register(player, password).ConfigureAwait(false));

            "Then the new Player should exists".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).Should().NotBeNull());

            "And the PlayerName should be correct".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).PlayerName.Should().Be(playerName));

            "And the Email should be correct".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).Email.Should().Be(email));

            "And the FirstName should be correct".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).FirstName.Should().Be(firstName));

            "And the LastName should be correct".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).LastName.Should().Be(lastName));

            "And the LastLogin should be null".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).LastLogin.Should().BeNull());

            "And the Role should be \"User\"".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).Role.Should().Be(PlayerRole.User));

            "And the ActivationDate should be null".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).ActivationDate.Should().BeNull());

            "And the IsActive flag should be false".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).IsActive.Should().BeFalse());

            "When trying to Login with the new Player".x(
                () => authenticationFunc = async() => await playerService.Authenticate(playerName, password).ConfigureAwait(false));

            "Then the Authentication should Throw an Exception".x(
                () => authenticationFunc.Should().Throw<Exception>().WithMessage("Your account is inactive. Please activate your account via the email you just received from us."));

            "When Activate the Player Account".x(
                async () => (await playerService.Activate(idString).ConfigureAwait(false)).Should().BeTrue());

            "Then the IsActive flag should be true".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).IsActive.Should().BeTrue());

            "And the ActivationDate flag should not be null".x(
                async () => (await playerService.GetById(idString).ConfigureAwait(false)).ActivationDate.Should().NotBeNull());

            "When trying to Login with the new Player again".x(
                async () => authPlayer = await playerService.Authenticate(playerName, password).ConfigureAwait(false));

            "Then the LastLogin value of the Player should not be null".x(() => authPlayer.LastLogin.Should().NotBeNull());
        }

        [Scenario]
        public void LoginWithWrongCredentials(Player player, Func<Task> authenticationFunc, Player authPlayer)
        {
            "Given a existing Player \"Admin\"".x(
                () => player = fakeData.FakePlayers.SingleOrDefault(p => p.Id == FakeData.AdminGuid));

            "When trying to Login with an empty Password".x(
                () => authenticationFunc = async () => await playerService.Authenticate(player.PlayerName, "").ConfigureAwait(false));

            "Then the Authentication should Throw an Exception".x(
                () => authenticationFunc.Should().Throw<Exception>().WithMessage("Username or password is empty."));

            "When trying to Login with an empty PlayerName".x(
                () => authenticationFunc = async () => await playerService.Authenticate("", FakeData.AdminPassword).ConfigureAwait(false));

            "Then the Authentication should Throw an Exception".x(
                () => authenticationFunc.Should().Throw<Exception>().WithMessage("Username or password is empty."));

            "When trying to Login with an not existing PlayerName".x(
                () => authenticationFunc = async () => await playerService.Authenticate("NotExistingPlayerName", FakeData.AdminPassword).ConfigureAwait(false));

            "Then the Authentication should Throw an Exception".x(
                () => authenticationFunc.Should().Throw<Exception>().WithMessage("Username or password is incorrect."));

            "When trying to Login with an incorrect Password".x(
                () => authenticationFunc = async () => await playerService.Authenticate(FakeData.AdminName, "WrongPassword").ConfigureAwait(false));

            "Then the Authentication should Throw an Exception".x(
                () => authenticationFunc.Should().Throw<Exception>().WithMessage("Username or password is incorrect."));
        }
    }
}
