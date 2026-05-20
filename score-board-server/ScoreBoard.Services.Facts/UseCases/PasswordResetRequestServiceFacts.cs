using System;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.Helpers;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class PasswordResetRequestServiceFacts
    {
        private readonly IPasswordResetRequestService testee;

        public PasswordResetRequestServiceFacts()
        {
            IEmailService emailService = A.Fake<IEmailService>();
            A.CallTo(() => emailService.SendPasswordResetEmail(null)).WithAnyArguments().Returns(Task.FromResult(true));

            testee = new PasswordResetRequestService(new PasswordResetRequestRepositoryFake(), emailService);
            testee.Initialize("");
        }

        [Fact]
        public async Task GetByExistingIdFact()
        {
            string id = FakeData.PasswordResetRequestId;
            PasswordResetRequest passwordResetRequest = await testee.GetById(id).ConfigureAwait(false);

            passwordResetRequest.Id.Should().Be(Guid.Parse(id));
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
            PasswordResetRequest passwordResetRequest = await testee.GetById(id).ConfigureAwait(false);

            passwordResetRequest.Should().BeNull();
        }

        [Fact]
        public async Task AddFact()
        {
            string id = "B64DEAB5-5C59-4C19-95FC-CE92C44A89E3";
            PasswordResetRequest passwordResetRequest = new PasswordResetRequest
            {
                Id = Guid.Parse(id),
                Player = new Player { Id = Guid.Parse(id) },
                TimeStamp = DateTime.Now
            };
            await testee.Add(passwordResetRequest).ConfigureAwait(false);
            PasswordResetRequest newPasswordResetRequest = await testee.GetById(id).ConfigureAwait(false);

            newPasswordResetRequest.Should().NotBeNull();
            newPasswordResetRequest.Player.Id.Should().Be(id);
        }

        [Fact]
        public async Task AddNullAsPasswordResetRequestFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Add(null).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Password Reset Request is required.");
        }

        [Fact]
        public async Task RemoveByIdFact()
        {
            await testee.Remove(FakeData.PasswordResetRequestId);
            PasswordResetRequest passwordResetRequest = await testee.GetById(FakeData.PasswordResetRequestId).ConfigureAwait(false);

            passwordResetRequest.Should().BeNull();
        }

        [Fact]
        public async Task RemoveNoValidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Remove("NotAValidId").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }

        [Fact]
        public async Task RemoveNoValidPasswordResetRequestFact()
        {
            await Task.Delay(0);
            PasswordResetRequest passwordResetRequest = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Func<Task> f = async () => await testee.Remove(passwordResetRequest).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Password Reset Request is required.");
        }

        [Fact]
        public async Task RemoveByPasswordResetRequestFact()
        {
            PasswordResetRequest passwordResetRequest = await testee.GetById(FakeData.PasswordResetRequestId).ConfigureAwait(false);
            await testee.Remove(passwordResetRequest);
            passwordResetRequest = await testee.GetById(FakeData.PasswordResetRequestId).ConfigureAwait(false);

            passwordResetRequest.Should().BeNull();
        }

        [Fact]
        public async Task SendPasswordResetEmailFact()
        {
            bool resendMailSuccessful = await testee.SendPasswordResetEmail(new PasswordResetRequest
            {
                Id = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                Player = new Player
                {
                    Id = Guid.NewGuid(),
                    PlayerName = "Test",
                    Email = "test"
                }
            }).ConfigureAwait(false);

            resendMailSuccessful.Should().BeTrue();
        }

        [Fact]
        public async Task SendPasswordResetEmailWithNullFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.SendPasswordResetEmail(null).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Password Reset Request is required.");
        }
    }
}
