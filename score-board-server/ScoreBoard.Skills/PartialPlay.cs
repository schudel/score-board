namespace ScoreBoard.Skills
{
    internal static class PartialPlay
    {
        public static double GetPartialPlayPercentage(object player)
        {
            // If the player doesn't support the interface, assume 1.0 == 100%
            if (!(player is ISupportPartialPlay partialPlay))
            {
                return 1.0;
            }

            double partialPlayPercentage = partialPlay.PartialPlayPercentage;

            // HACK to get around bug near 0
            const double smallestPercentage = 0.0001;
            if (partialPlayPercentage < smallestPercentage)
            {
                partialPlayPercentage = smallestPercentage;
            }

            return partialPlayPercentage;
        }
    }
}