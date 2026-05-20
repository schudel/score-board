using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.Helpers;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class PlayerServiceFacts
    {
        private readonly IPlayerService testee;

        public PlayerServiceFacts()
        {
            IPlayerRepository playerRepository = new PlayerRepositoryFake();
            IEmailService emailService = A.Fake<IEmailService>();
            A.CallTo(() => emailService.SendActivationEmail(null)).WithAnyArguments().Returns(Task.FromResult(true));
            A.CallTo(() => emailService.ResendActivationEmail(null)).WithAnyArguments().Returns(Task.FromResult(true));

            testee = new PlayerService(playerRepository, 
                new PasswordService(), 
                emailService, 
                new SettingsService(playerRepository), 
                new RatingService(new RatingRepositoryFake(), 
                    new MatchRepositoryFake(), 
                    new TeamService(new TeamRepositoryFake()), 
                    new RatingHistoryService(new RatingHistoryRepositoryFake())));
            testee.Initialize("");
        }

        [Theory]
        [InlineData("", "", "Username or password is empty.")]
        [InlineData("a", "a", "Username or password is incorrect.")]
        [InlineData("Administrator", "", "Username or password is empty.")]
        [InlineData("Not a Real User", "password", "Username or password is incorrect.")]
        [InlineData("Administrator", "WrongPassword", "Username or password is incorrect.")]
        [InlineData("Player 2", "password", "Your account is inactive. Please activate your account via the email you just received from us.")]
        public async Task AuthenticateWithWrongCredentialsFact(string playerName, string password, string exceptionText)
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Authenticate(playerName, password).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage(exceptionText);
        }

        [Fact]
        public async Task AuthenticateFact()
        {
            Player player = await testee.Authenticate(FakeData.AdminName, FakeData.AdminPassword).ConfigureAwait(false);

            player.Should().NotBeNull();
            player.Id.Should().Be(FakeData.AdminId);
            player.PlayerName.Should().Be(FakeData.AdminName);
            player.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterFact()
        {
            Player player = new Player
            {
                PlayerName = "Player XYZ",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "benjamin@schudel.rocks"
            };
            bool registered = await testee.Register(player, "password1234").ConfigureAwait(false);

            registered.Should().BeTrue();
        }

        [Fact]
        public async Task ActivateFact()
        {
            await Task.Delay(0);
            bool activated = await testee.Activate(FakeData.AdminId).ConfigureAwait(false);

            activated.Should().BeTrue();
        }
        
        [Fact]
        public async Task ActivateWithNoValidIdFact()
        {
            await Task.Delay(0);
            const string id = "NotAValidId";
            Func<Task> f = async () => await testee.Activate(id).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }
        
        [Fact]
        public async Task ActivateWithNonExistingIdFact()
        {
            string id = "40F3089B-1219-4AB6-B371-B612BB6CF0C0";
            bool activated = await testee.Activate(id).ConfigureAwait(false);

            activated.Should().BeFalse();
        }

        [Fact]
        public async Task ResendEmailFact()
        {
            bool resendMailSuccessful = await testee.ResendEmail(FakeData.AdminId).ConfigureAwait(false);

            resendMailSuccessful.Should().BeTrue();
        }
        
        [Fact]
        public async Task ResendEmailWithNoValidIdFact()
        {
            await Task.Delay(0);
            const string id = "NotAValidId";
            Func<Task> f = async () => await testee.ResendEmail(id).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }
        
        [Fact]
        public async Task ResendEmailWithNonExistingIdFact()
        {
            string id = "40F3089B-1219-4AB6-B371-B612BB6CF0C0";
            bool activated = await testee.ResendEmail(id).ConfigureAwait(false);

            activated.Should().BeFalse();
        }

        [Fact]
        public async Task GetByExistingIdFact()
        {
            string id = FakeData.AdminId;
            Player player = await testee.GetById(id).ConfigureAwait(false);

            player.Id.Should().Be(Guid.Parse(id));
        }

        [Fact]
        public async Task GetByNoValidIdFact()
        {
            await Task.Delay(0);
            const string id = "NotAValidId";
            Func<Task> f = async () => await testee.GetById(id).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }

        [Fact]
        public async Task GetByNonExistingIdFact()
        {
            string id = "40F3089B-1219-4AB6-B371-B612BB6CF0C0";
            Player player = await testee.GetById(id).ConfigureAwait(false);

            player.Should().BeNull();
        }

        [Fact]
        public async Task GetAllFact()
        {
            ICollection<Player> players = await testee.GetAll().ConfigureAwait(false);

            players.Should().NotBeNull();
            players.Should().HaveCount(4);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CreateFact(bool isActive)
        {
            Player player = new Player
            {
                Id = Guid.NewGuid(),
                PlayerName = "A New Player",
                Email = "benjamin@schudel.rocks",
                Role = null
            };
            Player newPlayer = await testee.Create(player, "password1234", isActive);

            newPlayer.Should().NotBeNull();
            newPlayer.Id.Should().Be(player.Id);
            newPlayer.PlayerName.Should().Be(player.PlayerName);
            newPlayer.Email.Should().Be(player.Email);
            newPlayer.PasswordHash.Should().NotBeNull();
            newPlayer.PasswordSalt.Should().NotBeNull();
            newPlayer.Role.Should().Be(PlayerRole.User);
            newPlayer.Settings.Should().BeEquivalentTo(SettingsService.CreateDefaultSettings(), options => options.Excluding(e => e.Id));
            if (isActive)
            {
                newPlayer.IsActive.Should().BeTrue();
                newPlayer.ActivationDate.Should().NotBeNull();
                newPlayer.RegistrationDate.Should().NotBeNull();
            }
            else
            {
                newPlayer.IsActive.Should().BeFalse();
                newPlayer.ActivationDate.Should().BeNull();
                newPlayer.RegistrationDate.Should().BeNull();
            }
        }

        [Fact]
        public async Task CreateWithPlayerNullFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Create(null, "password1234").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Player is required.");
        }

        [Fact]
        public async Task CreateWithEmptyPlayerNameFact()
        {
            await Task.Delay(0);
            Player p = new Player { Id = Guid.NewGuid() };
            Func<Task> f = async () => await testee.Create(p, "password1234").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("PlayerName is required.");
        }
        
        [Fact]
        public async Task CreateWithEmptyPasswordFact()
        {
            await Task.Delay(0);
            Player p = new Player { Id = Guid.NewGuid(), PlayerName = "A Player"};
            Func<Task> f = async () => await testee.Create(p, "").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Password is required.");
        }
        
        [Fact]
        public async Task CreateWithEmptyEmailFact()
        {
            await Task.Delay(0);
            Player p = new Player { Id = Guid.NewGuid(), PlayerName = "A Player"};
            Func<Task> f = async () => await testee.Create(p, "password1234").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Email is required.");
        }
        
        [Fact]
        public async Task CreateWithNotValidEmailFact()
        {
            await Task.Delay(0);
            Player p = new Player { Id = Guid.NewGuid(), PlayerName = "A Player", Email = "NotAValidEmail"};
            Func<Task> f = async () => await testee.Create(p, "password1234").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Email Address is not valid.");
        }
        
        [Fact]
        public async Task CreateWithAlreadyExistingPlayerNameFact()
        {
            await Task.Delay(0);
            Player p = new Player { Id = Guid.NewGuid(), PlayerName = FakeData.AdminName, Email = "benjamin@schudel.rocks" };
            Func<Task> f = async () => await testee.Create(p, "password1234").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("PlayerName \"" + FakeData.AdminName + "\" is already taken.");
        }

        [Fact]
        public async Task CreateWithAlreadyExistingEmailFact()
        {
            await Task.Delay(0);
            Player p = new Player { Id = Guid.NewGuid(), PlayerName = "TotallyNewName", Email = FakeData.AdminEmail };
            Func<Task> f = async () => await testee.Create(p, "password1234").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Email \"" + FakeData.AdminEmail + "\" is already taken.");
        }

        [Fact]
        public async Task UpdateFact()
        {
            Player newPlayer = new Player
            {
                PlayerName = "New PlayerName",
                Email = "new@email.com",
                Image = "newImage"
            };
            await testee.Update(FakeData.AdminId, newPlayer).ConfigureAwait(false);
            Player player = await testee.GetById(FakeData.AdminId).ConfigureAwait(false);
            
            player.PlayerName.Should().Be(newPlayer.PlayerName);
            player.Email.Should().Be(newPlayer.Email);
            player.Image.Should().Be(newPlayer.Image);
        }
        
        [Fact]
        public async Task UpdateWithInvalidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update("NoValidId", new Player()).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NoValidId\"");
        }
        
        [Fact]
        public async Task UpdateWithNonExistingIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update("0EF28727-EBA4-46C1-B5C7-3B2A5F300106", new Player()).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Player not found");
        }
        
        [Fact]
        public async Task UpdateWithAlreadyExistingPlayerNameFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update(FakeData.AdminId, new Player{PlayerName = FakeData.Player2Name}).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("PlayerName \"" + FakeData.Player2Name + "\" is already taken");
        }

        [Fact]
        public async Task RemoveWithExistingIdFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);
            await testee.Remove(FakeData.AdminId);
            long newCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(newCount + 1);
        }
        
        [Fact]
        public async Task RemoveWithExistingPlayerFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);
            Player player = await testee.GetById(FakeData.AdminId).ConfigureAwait(false);
            await testee.Remove(player);
            long newCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(newCount + 1);
        }
        
        [Fact]
        public async Task RemoveWithPlayerNullFact()
        {
            await Task.Delay(0);
            Player p = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Func<Task> f = async () => await testee.Remove(p).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Player is required.");
        }

        [Fact]
        public async Task RemoveWithInvalidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Remove("NotAValidId").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }
        
        [Fact]
        public async Task CountFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(4);
        }
        
        [Fact]
        public async Task ChangePasswordFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ChangePassword(FakeData.AdminId, FakeData.AdminPassword, "NewPassword!");

            f.Should().NotThrow();
        }
        
        [Fact]
        public async Task ChangePasswordWithInvalidNewPasswordFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ChangePassword(FakeData.AdminId, FakeData.AdminPassword, "").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("New Password is required");
        }
        
        [Fact]
        public async Task ChangePasswordWithInvalidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ChangePassword("NotAValidId", FakeData.AdminPassword, "NewPassword!").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }
        
        [Fact]
        public async Task ChangePasswordWithNonExistingIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ChangePassword("69DF75F5-985A-4344-ADBD-3A6D43F4403B", FakeData.AdminPassword, "NewPassword!").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Player not found");
        }
        
        [Fact]
        public async Task ChangePasswordWithIncorrectCurrentPasswordFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ChangePassword(FakeData.AdminId, "Wrong Password!", "NewPassword!").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("The current Password is not correct");
        }

        [Fact]
        public async Task ResetPasswordFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ResetPassword(FakeData.AdminId, "NewPassword!");

            f.Should().NotThrow();
        }

        [Fact]
        public async Task ResetPasswordWithInvalidNewPasswordFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ResetPassword(FakeData.AdminId, "").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("New Password is required");
        }

        [Fact]
        public async Task ResetPasswordWithInvalidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ResetPassword("NotAValidId", "NewPassword!").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }

        [Fact]
        public async Task ResetPasswordWithNonExistingIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.ResetPassword("69DF75F5-985A-4344-ADBD-3A6D43F4403B", "NewPassword!").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Player not found");
        }

        [Fact]
        public async Task GetByExistingEmailFact()
        {
            string email = FakeData.AdminEmail;
            Player player = await testee.GetByEmail(email).ConfigureAwait(false);

            player.Email.Should().Be(email);
        }

        [Fact]
        public async Task GetByEmptyEmailFact()
        {
            await Task.Delay(0);
            const string email = "";
            Func<Task> f = async () => await testee.GetByEmail(email).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("E-Mail Address is required");
        }

        [Fact]
        public async Task GetByNonExistingEmailFact()
        {
            string email = "notan@existingemail.com";
            Player player = await testee.GetByEmail(email).ConfigureAwait(false);

            player.Should().BeNull();
        }
    }
}
