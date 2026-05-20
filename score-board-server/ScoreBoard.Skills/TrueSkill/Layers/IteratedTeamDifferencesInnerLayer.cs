using System;
using System.Collections.Generic;
using System.Linq;
using ScoreBoard.Skills.FactorGraphs;
using ScoreBoard.Skills.Numerics;
using ScoreBoard.Skills.TrueSkill.Factors;

namespace ScoreBoard.Skills.TrueSkill.Layers
{
    // The whole purpose of this is to do a loop on the bottom
    internal class IteratedTeamDifferencesInnerLayer<TPlayer> :
        TrueSkillFactorGraphLayer
            <TPlayer, Variable<GaussianDistribution>, GaussianWeightedSumFactor, Variable<GaussianDistribution>>
    {
        private readonly TeamDifferencesComparisonLayer<TPlayer> teamDifferencesComparisonLayer;

        private readonly TeamPerformancesToTeamPerformanceDifferencesLayer<TPlayer>
            teamPerformancesToTeamPerformanceDifferencesLayer;

        public IteratedTeamDifferencesInnerLayer(TrueSkillFactorGraph<TPlayer> parentGraph,
                                                 TeamPerformancesToTeamPerformanceDifferencesLayer<TPlayer>
                                                     teamPerformancesToPerformanceDifferences,
                                                 TeamDifferencesComparisonLayer<TPlayer> teamDifferencesComparisonLayer)
            : base(parentGraph)
        {
            teamPerformancesToTeamPerformanceDifferencesLayer = teamPerformancesToPerformanceDifferences;
            this.teamDifferencesComparisonLayer = teamDifferencesComparisonLayer;
        }

        public override IEnumerable<Factor<GaussianDistribution>> UntypedFactors =>
            teamPerformancesToTeamPerformanceDifferencesLayer.UntypedFactors.Concat(
                teamDifferencesComparisonLayer.UntypedFactors);

        public override void BuildLayer()
        {
            teamPerformancesToTeamPerformanceDifferencesLayer.SetRawInputVariablesGroups(InputVariablesGroups);
            teamPerformancesToTeamPerformanceDifferencesLayer.BuildLayer();

            teamDifferencesComparisonLayer.SetRawInputVariablesGroups(
                teamPerformancesToTeamPerformanceDifferencesLayer.GetRawOutputVariablesGroups());
            teamDifferencesComparisonLayer.BuildLayer();
        }

        public override Schedule CreatePriorSchedule()
        {
            Schedule loop;

            switch (InputVariablesGroups.Count)
            {
                case 0:
                case 1:
                    throw new InvalidOperationException();
                case 2:
                    loop = CreateTwoTeamInnerPriorLoopSchedule();
                    break;
                default:
                    loop = CreateMultipleTeamInnerPriorLoopSchedule();
                    break;
            }

            // When dealing with differences, there are always (n-1) differences, so add in the 1
            int totalTeamDifferences = teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors.Count;
            //int totalTeams = totalTeamDifferences + 1;

            var innerSchedule = new ScheduleSequence<GaussianDistribution>(
                "inner schedule",
                new[]
                    {
                        loop,
                        new ScheduleStep<GaussianDistribution>(
                            "teamPerformanceToPerformanceDifferenceFactors[0] @ 1",
                            teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors[0], 1),
                        new ScheduleStep<GaussianDistribution>(
                            $"teamPerformanceToPerformanceDifferenceFactors[teamTeamDifferences = {totalTeamDifferences} - 1] @ 2",
                            teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors[totalTeamDifferences - 1], 2)
                    }
                );

            return innerSchedule;
        }

        private Schedule CreateTwoTeamInnerPriorLoopSchedule()
        {
            return ScheduleSequence(
                new[]
                    {
                        new ScheduleStep<GaussianDistribution>(
                            "send team perf to perf differences",
                            teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors[0],
                            0),
                        new ScheduleStep<GaussianDistribution>(
                            "send to greater than or within factor",
                            teamDifferencesComparisonLayer.LocalFactors[0],
                            0)
                    },
                "loop of just two teams inner sequence");
        }

        private Schedule CreateMultipleTeamInnerPriorLoopSchedule()
        {
            int totalTeamDifferences = teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors.Count;

            var forwardScheduleList = new List<Schedule>();

            for (int i = 0; i < totalTeamDifferences - 1; i++)
            {
                Schedule currentForwardSchedulePiece =
                    ScheduleSequence(
                        new Schedule[]
                            {
                                new ScheduleStep<GaussianDistribution>(
                                    $"team perf to perf diff {i}",
                                    teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors[i], 0),
                                new ScheduleStep<GaussianDistribution>(
                                    $"greater than or within result factor {i}",
                                    teamDifferencesComparisonLayer.LocalFactors[i],
                                    0),
                                new ScheduleStep<GaussianDistribution>(
                                    $"team perf to perf diff factors [{i}], 2",
                                    teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors[i], 2)
                            }, "current forward schedule piece {0}", i);

                forwardScheduleList.Add(currentForwardSchedulePiece);
            }

            var forwardSchedule =
                new ScheduleSequence<GaussianDistribution>(
                    "forward schedule",
                    forwardScheduleList);

            var backwardScheduleList = new List<Schedule>();

            for (int i = 0; i < totalTeamDifferences - 1; i++)
            {
                var currentBackwardSchedulePiece = new ScheduleSequence<GaussianDistribution>(
                    "current backward schedule piece",
                    new Schedule[]
                        {
                            new ScheduleStep<GaussianDistribution>(
                                $"teamPerformanceToPerformanceDifferenceFactors[totalTeamDifferences - 1 - {i}] @ 0",
                                teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors[
                                    totalTeamDifferences - 1 - i], 0),
                            new ScheduleStep<GaussianDistribution>(
                                $"greaterThanOrWithinResultFactors[totalTeamDifferences - 1 - {i}] @ 0",
                                teamDifferencesComparisonLayer.LocalFactors[totalTeamDifferences - 1 - i], 0),
                            new ScheduleStep<GaussianDistribution>(
                                $"teamPerformanceToPerformanceDifferenceFactors[totalTeamDifferences - 1 - {i}] @ 1",
                                teamPerformancesToTeamPerformanceDifferencesLayer.LocalFactors[
                                    totalTeamDifferences - 1 - i], 1)
                        }
                    );
                backwardScheduleList.Add(currentBackwardSchedulePiece);
            }

            var backwardSchedule =
                new ScheduleSequence<GaussianDistribution>(
                    "backward schedule",
                    backwardScheduleList);

            var forwardBackwardScheduleToLoop =
                new ScheduleSequence<GaussianDistribution>(
                    "forward Backward Schedule To Loop",
                    new Schedule[]
                        {
                            forwardSchedule, backwardSchedule
                        });

            const double initialMaxDelta = 0.0001;

            var loop = new ScheduleLoop(
                $"loop with max delta of {initialMaxDelta}",
                forwardBackwardScheduleToLoop,
                initialMaxDelta);

            return loop;
        }
    }
}