
namespace ScoreBoard.Skills.Elo
{
    public class KFactor
    {
        private readonly double value;

        protected KFactor()
        {
        }

        public KFactor(double exactKFactor)
        {
            value = exactKFactor;
        }

        public virtual double GetValueForRating(double rating)
        {
            return value;
        }
    }
}
