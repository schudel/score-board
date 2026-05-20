using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class GameServiceFacts
    {
        private readonly IGameService testee;

        public GameServiceFacts()
        {
            IGameRepository gameRepository = new GameRepositoryFake();

            testee = new GameService(gameRepository);
            testee.Initialize("");
        }

        [Fact]
        public async Task GetByExistingIdFact()
        {
            string id = FakeData.GameFifaId;
            Game game = await testee.GetById(id).ConfigureAwait(false);

            game.Id.Should().Be(Guid.Parse(id));
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
            Game game = await testee.GetById(id).ConfigureAwait(false);

            game.Should().BeNull();
        }

        [Fact]
        public async Task GetAllFact()
        {
            ICollection<Game> games = await testee.GetAll().ConfigureAwait(false);

            games.Should().NotBeNull();
            games.Should().HaveCount(3);
        }

        [Fact]
        public async Task AddFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);
            await testee.Add(new Game {Id = Guid.NewGuid(), Name = "Test-Game"});
            long newCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(newCount - 1);
        }

        [Fact]
        public async Task AddNullAsGameFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Add(null).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Game is required.");
        }

        [Fact]
        public async Task UpdateFact()
        {
            string id = FakeData.GameFifaId;
            Game game = await testee.GetById(id).ConfigureAwait(false);
            const string name = "Not FIFA anymore";
            game.Name = name;
            await testee.Update(id, game).ConfigureAwait(false);
            game = await testee.GetById(id).ConfigureAwait(false);

            game.Name.Should().Be(name);
        }

        [Fact]
        public async Task UpdateNoValidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update("NoValidId", new Game()).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NoValidId\"");
        }

        [Fact]
        public async Task UpdateNonExistingIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update("0EF28727-EBA4-46C1-B5C7-3B2A5F300106", new Game()).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Game not found");
        }

        [Fact]
        public async Task RemoveByIdFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);
            await testee.Remove(FakeData.GameFifaId);
            long newCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(newCount + 1);
        }

        [Fact]
        public async Task RemoveNoValidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Remove("NotAValidId").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }

        [Fact]
        public async Task RemoveNoValidGameFact()
        {
            await Task.Delay(0);
            Game game = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Func<Task> f = async () => await testee.Remove(game).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Game is required.");
        }

        [Fact]
        public async Task RemoveByGameFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);
            Game game = await testee.GetById(FakeData.GameFifaId).ConfigureAwait(false);
            await testee.Remove(game);
            long newCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(newCount + 1);
        }

        [Fact]
        public async Task CountFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(3);
        }

        [Fact]
        public async Task GetGenresFact()
        {
            ICollection<string> genres = await testee.GetGenres().ConfigureAwait(false);

            genres.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetTypesFact()
        {
            ICollection<string> types = await testee.GetTypes().ConfigureAwait(false);

            types.Should().HaveCount(3);
        }
    }
}
