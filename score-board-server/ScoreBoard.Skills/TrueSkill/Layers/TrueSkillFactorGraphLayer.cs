using ScoreBoard.Skills.FactorGraphs;
using ScoreBoard.Skills.Numerics;

namespace ScoreBoard.Skills.TrueSkill.Layers
{
    internal abstract class TrueSkillFactorGraphLayer<TPlayer, TInputVariable, TFactor, TOutputVariable>
        :
            FactorGraphLayer
                <TrueSkillFactorGraph<TPlayer>, GaussianDistribution, Variable<GaussianDistribution>, TInputVariable,
                TFactor, TOutputVariable>
        where TInputVariable : Variable<GaussianDistribution>
        where TFactor : Factor<GaussianDistribution>
        where TOutputVariable : Variable<GaussianDistribution>
    {
        protected TrueSkillFactorGraphLayer(TrueSkillFactorGraph<TPlayer> parentGraph)
            : base(parentGraph)
        {
        }
    }
}