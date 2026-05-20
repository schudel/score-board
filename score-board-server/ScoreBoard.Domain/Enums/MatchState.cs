using System.Collections.Generic;

namespace ScoreBoard.Domain.Enums
{
    public class MatchState
    {
        private static readonly Dictionary<MatchStateEnum, MatchState> Dictionary = new Dictionary<MatchStateEnum, MatchState>();

        private MatchState(string name, MatchStateEnum matchStateEnum)
        {
            Name = name;
            MatchStateEnum = matchStateEnum;
            Dictionary.Add(matchStateEnum, this);
        }

        public string Name { get; }
        public MatchStateEnum MatchStateEnum { get; }

        public static IList<MatchState> GetAllMatchStates
        {
            get
            {
                IList<MatchState> allMatchState = new List<MatchState>();
                foreach (KeyValuePair<MatchStateEnum, MatchState> matchState in Dictionary)
                {
                    allMatchState.Add(matchState.Value);
                }
                return allMatchState;
            }
        }

        public static MatchState GetMatchState(MatchStateEnum matchStateEnum)
        {
            foreach (var (key, matchState) in Dictionary)
            {
                if (key == matchStateEnum)
                {
                    return matchState;
                }
            }
            return null;
        }

        public static MatchState GetMatchState(string matchStateString)
        {
            foreach (var (_, matchState) in Dictionary)
            {
                if (matchState.Name == matchStateString)
                {
                    return matchState;
                }
            }
            return null;
        }

        public static readonly MatchState Scheduled = new MatchState("Scheduled", MatchStateEnum.Scheduled);
        public static readonly MatchState Playing = new MatchState("Playing", MatchStateEnum.Playing);
        public static readonly MatchState Done = new MatchState("Done", MatchStateEnum.Done);
        public static readonly MatchState Interrupted = new MatchState("Interrupted", MatchStateEnum.Interrupted);
        public static readonly MatchState Aborted = new MatchState("Aborted", MatchStateEnum.Aborted);
    }

    public enum MatchStateEnum
    {
        Scheduled = 0,
        Playing = 1,
        Done = 2,
        Interrupted = 3,
        Aborted = 4
    }
}
