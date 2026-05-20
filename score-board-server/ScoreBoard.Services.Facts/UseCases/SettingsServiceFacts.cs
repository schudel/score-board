using System;
using System.Threading.Tasks;
using FluentAssertions;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class SettingsServiceFacts
    {
        private readonly ISettingsService testee;

        public SettingsServiceFacts()
        {
            testee = new SettingsService(new PlayerRepositoryFake());
            testee.Initialize("");
        }

        [Fact]
        public void CreateDefaultSettingsFact()
        {
            Settings defaultSettings = SettingsService.CreateDefaultSettings();

            defaultSettings.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateFact()
        {
            await Task.Delay(0);
            Func<Task> f = async() => await testee.Update(FakeData.AdminId, new Settings()).ConfigureAwait(false);

            f.Should().NotThrow();
        }

        [Fact]
        public async Task UpdateNoValidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update("NoValidId", new Settings()).ConfigureAwait(false);

            f.Should().Throw<Exception>();
        }

        [Fact]
        public async Task UpdateNonExistingIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update("0EF28727-EBA4-46C1-B5C7-3B2A5F300106", new Settings()).ConfigureAwait(false);

            f.Should().Throw<Exception>();
        }
    }
}
