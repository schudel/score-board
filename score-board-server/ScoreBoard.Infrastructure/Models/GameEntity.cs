using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class GameEntity
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
        public virtual string Genre { get; set; }
        public virtual string Image { get; set; }
        public virtual double Beta { get; set; }
        public virtual double DrawProbability { get; set; }
        public virtual double DynamicsFactor { get; set; }
        public virtual double InitialConservativeRating { get; set; }
        public virtual double InitialMean { get; set; }
        public virtual double InitialStandardDeviation { get; set; }

        public virtual Game GetGame(bool slim = false)
        {
            Game game = new Game
            {
                Id = Id,
                Name = Name,
                Type = Type,
                Genre = Genre,
                Beta = Beta,
                DrawProbability = DrawProbability,
                DynamicsFactor = DynamicsFactor,
                InitialConservativeRating = InitialConservativeRating,
                InitialMean = InitialMean,
                InitialStandardDeviation = InitialStandardDeviation
            };
            if (!slim)
            {
                game.Image = Image;
            }
            return game;
        }

        public static GameEntity Create(Game game)
        {
            if (game == null)
            {
                return null;
            }
            GameEntity gameEntity = new GameEntity
            {
                Id = game.Id,
                Name = game.Name,
                Type = game.Type,
                Genre = game.Genre,
                Image = game.Image,
                Beta = game.Beta,
                DrawProbability = game.DrawProbability,
                DynamicsFactor = game.DynamicsFactor,
                InitialConservativeRating = game.InitialConservativeRating,
                InitialMean = game.InitialMean,
                InitialStandardDeviation = game.InitialStandardDeviation
            };
            return gameEntity;
        }
    }
}
