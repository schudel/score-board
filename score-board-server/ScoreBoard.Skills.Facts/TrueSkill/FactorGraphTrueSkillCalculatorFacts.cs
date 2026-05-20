using ScoreBoard.Skills.TrueSkill;
using Xunit;

namespace ScoreBoard.Skills.Facts.TrueSkill
{
    public class FactorGraphTrueSkillCalculatorFacts
    {
        [Fact]
        public void FullFactorGraphCalculatorFact()
        {
            var calculator = new FactorGraphTrueSkillCalculator();

            // We can test all classes 
            TrueSkillCalculatorFacts.TestAllTwoPlayerScenarios(calculator);
            TrueSkillCalculatorFacts.TestAllTwoTeamScenarios(calculator);
            TrueSkillCalculatorFacts.TestAllMultipleTeamScenarios(calculator);

            TrueSkillCalculatorFacts.TestPartialPlayScenarios(calculator);
        }
    }
}