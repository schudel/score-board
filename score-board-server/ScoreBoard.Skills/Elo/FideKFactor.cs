
namespace ScoreBoard.Skills.Elo
{
    // see http://ratings.fide.com/calculator_rtd.phtml for details
    public class FideKFactor : KFactor
    {
        public override double GetValueForRating(double rating)
        {
            return rating < 2400 ? 15 : 10;
        }

        /// <summary>
        /// Indicates someone who has played less than 30 games.
        /// </summary>        
        public class Provisional : FideKFactor
        {
            public override double GetValueForRating(double rating)
            {
                return 25;
            }
        }
    }
}
