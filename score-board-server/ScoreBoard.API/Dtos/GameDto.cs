using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class GameDto
    {
        public string Id { get; set; }

        //[Required]
        public string Name { get; set; }

        //[Required]
        public string Type { get; set; }

        //[Required]
        public string Genre { get; set; }

        public string Image { get; set; }
        public double Beta { get; set; }
        public double DrawProbability { get; set; }
        public double DynamicsFactor { get; set; }
        public double InitialConservativeRating { get; set; }
        public double InitialMean { get; set; }
        public double InitialStandardDeviation { get; set; }

        public Game GetGame()
        {
            if (Guid.TryParse(Id, out Guid guid))
            {
                return new Game
                {
                    Id = guid,
                    Name = Name,
                    Genre = Genre,
                    Type = Type,
                    Image = Image,
                    Beta = Beta,
                    DrawProbability = DrawProbability,
                    DynamicsFactor = DynamicsFactor,
                    InitialConservativeRating = InitialConservativeRating,
                    InitialMean = InitialMean,
                    InitialStandardDeviation = InitialStandardDeviation
                };
            }
            throw new Exception("No valid Id: \"" + Id + "\"");
        }

        public static GameDto Create(Game game)
        {
            if (game == null)
            {
                return null;
            }
            GameDto gameDto = new GameDto
            {
                Id = game.Id.ToString(),
                Name = game.Name,
                Genre = game.Genre,
                Type = game.Type,
                Image = game.Image,
                Beta = game.Beta,
                DrawProbability = game.DrawProbability,
                DynamicsFactor = game.DynamicsFactor,
                InitialConservativeRating = game.InitialConservativeRating,
                InitialMean = game.InitialMean,
                InitialStandardDeviation = game.InitialStandardDeviation
            };
            return gameDto;
        }
    }
}
