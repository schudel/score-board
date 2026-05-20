using ScoreBoard.Services.UseCases;
using Xbehave;

namespace ScoreBoard.Services.Specs
{
    public class RatingSpecs
    {
        private const double ErrorTolerance = .000001;
        private IRatingService ratingService;

        [Background]
        // ReSharper disable once UnusedMember.Global
        public void Background()
        {
            // TODO: implement
        }
    }
}
