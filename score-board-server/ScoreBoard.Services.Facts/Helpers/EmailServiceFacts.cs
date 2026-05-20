using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes;
using ScoreBoard.Services.Helpers;
using Xunit;

namespace ScoreBoard.Services.Facts.Helpers
{
    public class EmailServiceFacts
    {
        private readonly IEmailService testee;
        private readonly FakeData fakeData;

        public EmailServiceFacts()
        {
            testee = new EmailService(Options.Create(new SmtpSettings()));
            fakeData = new FakeData();
        }

        [Trait("Category", "Explicit")]
        [Fact]
        public async Task SendActivationEmailFact()
        {
            Player player = fakeData.FakePlayers.SingleOrDefault(p => p.Id == FakeData.AdminGuid);
            bool success = await testee.SendActivationEmail(player).ConfigureAwait(false);

            success.Should().BeTrue();
        }

        [Trait("Category", "Explicit")]
        [Fact]
        public async Task SendActivationEmailWithoutPlayerFact()
        {
            bool success = await testee.SendActivationEmail(null).ConfigureAwait(false);

            success.Should().BeFalse();
        }

        [Trait("Category", "Explicit")]
        [Fact]
        public async Task ResendActivationEmailFact()
        {
            Player player = fakeData.FakePlayers.SingleOrDefault(p => p.Id == FakeData.AdminGuid);
            bool success = await testee.ResendActivationEmail(player).ConfigureAwait(false);

            success.Should().BeTrue();
        }

        [Trait("Category", "Explicit")]
        [Fact]
        public async Task ResendActivationEmailWithoutPlayerFact()
        {
            bool success = await testee.ResendActivationEmail(null).ConfigureAwait(false);

            success.Should().BeFalse();
        }

        [Trait("Category", "Explicit")]
        [Fact]
        public async Task SendInvitationFact()
        {
            Player player = fakeData.FakePlayers.SingleOrDefault(p => p.Id == FakeData.AdminGuid);
            bool success = player != null && await testee.SendInvitation(FakeData.Player2Name, FakeData.AdminName, new []{ player.Email}, "blub").ConfigureAwait(false);

            success.Should().BeTrue();
        }

        [Trait("Category", "Explicit")]
        [Fact]
        public async Task SendInvitationWithoutReceiverEmailFact()
        {
            bool success = await testee.SendInvitation(FakeData.Player2Name, FakeData.AdminName, null, "blub").ConfigureAwait(false);

            success.Should().BeFalse();
        }

        [Trait("Category", "Explicit")]
        [Fact]
        public async Task SendPasswordResetEmailFact()
        {
            PasswordResetRequest passwordResetRequest = fakeData.FakePasswordResetRequests[0];
            bool success = passwordResetRequest != null && await testee.SendPasswordResetEmail(passwordResetRequest).ConfigureAwait(false);

            success.Should().BeTrue();
        }

        [Trait("Category", "Explicit")]
        [Fact]
        public async Task SendPasswordResetEmailWithNullFact()
        {
            bool success = await testee.SendPasswordResetEmail(null).ConfigureAwait(false);

            success.Should().BeFalse();
        }
    }
}
