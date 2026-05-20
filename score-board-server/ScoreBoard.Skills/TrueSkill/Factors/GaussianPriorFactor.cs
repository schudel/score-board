using System;
using ScoreBoard.Skills.FactorGraphs;
using ScoreBoard.Skills.Numerics;

namespace ScoreBoard.Skills.TrueSkill.Factors
{
    /// <summary>
    /// Supplies the factor graph with prior information.
    /// </summary>
    /// <remarks>See the accompanying math paper for more details.</remarks>
    public class GaussianPriorFactor : GaussianFactor
    {
        private readonly GaussianDistribution newMessage;

        public GaussianPriorFactor(double mean, double variance, Variable<GaussianDistribution> variable)
            : base($"Prior value going to {variable}")
        {
            newMessage = new GaussianDistribution(mean, Math.Sqrt(variance));
            CreateVariableToMessageBinding(variable,
                                           new Message<GaussianDistribution>(
                                               GaussianDistribution.FromPrecisionMean(0, 0), "message from {0} to {1}",
                                               this, variable));
        }

        protected override double UpdateMessage(Message<GaussianDistribution> message,
                                                Variable<GaussianDistribution> variable)
        {
            GaussianDistribution oldMarginal = variable.Value.Clone();
            Message<GaussianDistribution> oldMessage = message;
            GaussianDistribution newMarginal =
                GaussianDistribution.FromPrecisionMean(
                    oldMarginal.PrecisionMean + newMessage.PrecisionMean - oldMessage.Value.PrecisionMean,
                    oldMarginal.Precision + newMessage.Precision - oldMessage.Value.Precision);
            variable.Value = newMarginal;
            message.Value = newMessage;
            return oldMarginal - newMarginal;
        }
    }
}