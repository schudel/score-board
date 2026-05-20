using ScoreBoard.Skills.TrueSkill;
using Xunit;

namespace ScoreBoard.Skills.Facts.TrueSkill
{
    public class TwoTeamTrueSkillCalculatorFacts
    {
        [Fact]
        public void TwoTeamTrueSkillCalculatorFact()
        {
            var calculator = new TwoTeamTrueSkillCalculator();

            // This calculator supports up to two teams with many players each
            TrueSkillCalculatorFacts.TestAllTwoPlayerScenarios(calculator);
            TrueSkillCalculatorFacts.TestAllTwoTeamScenarios(calculator);
        }
    }
}