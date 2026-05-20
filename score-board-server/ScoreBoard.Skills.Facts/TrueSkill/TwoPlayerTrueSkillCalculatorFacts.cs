using ScoreBoard.Skills.TrueSkill;
using Xunit;

namespace ScoreBoard.Skills.Facts.TrueSkill
{
    public class TwoPlayerTrueSkillCalculatorFacts
    {
        [Fact]
        public void TwoPlayerTrueSkillCalculatorFact()
        {
            var calculator = new TwoPlayerTrueSkillCalculator();

            // We only support two players
            TrueSkillCalculatorFacts.TestAllTwoPlayerScenarios(calculator);

            // TODO: Assert failures for larger teams
        }    
    }
}