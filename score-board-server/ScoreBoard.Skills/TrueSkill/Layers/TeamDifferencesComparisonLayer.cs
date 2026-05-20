using ScoreBoard.Skills.FactorGraphs;
using ScoreBoard.Skills.Numerics;
using ScoreBoard.Skills.TrueSkill.Factors;

namespace ScoreBoard.Skills.TrueSkill.Layers
{
    internal class TeamDifferencesComparisonLayer<TPlayer> :
        TrueSkillFactorGraphLayer
            <TPlayer, Variable<GaussianDistribution>, GaussianFactor, DefaultVariable<GaussianDistribution>>
    {
        private readonly double epsilon;
        private readonly int[] teamRanks;

        public TeamDifferencesComparisonLayer(TrueSkillFactorGraph<TPlayer> parentGraph, int[] teamRanks)
            : base(parentGraph)
        {
            this.teamRanks = teamRanks;
            GameInfo gameInfo = ParentFactorGraph.GameInfo;
            epsilon = DrawMargin.GetDrawMarginFromDrawProbability(gameInfo.DrawProbability, gameInfo.Beta);
        }

        public override void BuildLayer()
        {
            for (int i = 0; i < InputVariablesGroups.Count; i++)
            {
                bool isDraw = teamRanks[i] == teamRanks[i + 1];
                Variable<GaussianDistribution> teamDifference = InputVariablesGroups[i][0];

                GaussianFactor factor =
                    isDraw
                        ? (GaussianFactor) new GaussianWithinFactor(epsilon, teamDifference)
                        : new GaussianGreaterThanFactor(epsilon, teamDifference);

                AddLayerFactor(factor);
            }
        }
    }
}