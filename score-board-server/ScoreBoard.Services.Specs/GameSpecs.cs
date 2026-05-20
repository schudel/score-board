using System;
using FluentAssertions;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.UseCases;
using Xbehave;

namespace ScoreBoard.Services.Specs
{
    public class GameSpecs
    {
        private IGameService gameService;
        private const double ErrorTolerance = .000001;

        [Background]
        // ReSharper disable once UnusedMember.Global
        public void Background()
        {
            "Create Game Service".x(
                () => gameService = new GameService(new GameRepositoryFake()));

            "Initialize Game Service".x(
                () => gameService.Initialize(""));
        }

        [Scenario]
        public void CreateNewGameAndUpdateValuesSpec(Game game)
        {
            const string idString = "42018CB9-B45D-4F4D-85F4-75EA29AA6437";
            Guid id = Guid.Parse(idString);
            const string name = "New Game";

            "Given a new Game with the name \"New Game\"".x(
                () => game = new Game { Id =id, Name = name });

            "When adding the new Game".x(
                async () => await gameService.Add(game).ConfigureAwait(false));

            "Then the Game should have thew name \"New Game\"".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).Name.Should().Be(name));

            "And the Genre value should be empty string".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).Genre.Should().BeNullOrEmpty());

            "And the Type value should be empty string".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).Type.Should().BeNullOrEmpty());

            "And the Image value should be empty string".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).Image.Should().BeNullOrEmpty());

            "And the Beta value should be 0".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).Beta.Should()
                    .BeApproximately(0, ErrorTolerance));

            "And the DrawProbability value should be 0".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).DrawProbability.Should()
                    .BeApproximately(0, ErrorTolerance));

            "And the DynamicsFactor value should be 0".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).DynamicsFactor.Should()
                    .BeApproximately(0, ErrorTolerance));

            "And the InitialConservativeRating value should be 0".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).InitialConservativeRating.Should()
                    .BeApproximately(0, ErrorTolerance));

            "And the InitialMean value should be 0".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).InitialMean.Should()
                    .BeApproximately(0, ErrorTolerance));

            "And the InitialStandardDeviation value should be 0".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).InitialStandardDeviation.Should()
                    .BeApproximately(0, ErrorTolerance));

            "When changing the Genre to \"Sport\"".x(
                () => game.Genre = "Sport");

            "And updating the Game".x(
                async() => await gameService.Update(idString, game).ConfigureAwait(false));

            "Then the Genre should now be \"Sport\"".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).Genre.Should().Be("Sport"));

            "When changing the Type to \"PS4\"".x(
                () => game.Type = "PS4");

            "And updating the Game".x(
                async () => await gameService.Update(idString, game).ConfigureAwait(false));

            "Then the Type should now be \"PS4\"".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).Type.Should().Be("PS4"));

            "When changing the InitialConservativeRating to \"25\"".x(
                () => game.InitialConservativeRating = 25);

            "And updating the Game".x(
                async () => await gameService.Update(idString, game).ConfigureAwait(false));

            "Then the InitialConservativeRating should now be \"25\"".x(
                async () => (await gameService.GetById(idString).ConfigureAwait(false)).InitialConservativeRating.Should()
                    .BeApproximately(25, ErrorTolerance));
        }
    }
}
