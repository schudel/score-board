using System;
using System.Globalization;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class LiveMatchDto
    {
        public string Id { get; set; }
        public string MatchId { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public string State { get; set; }
        public string TimeStamp { get; set; }

        public LiveMatch GetLiveMatch()
        {
            if (!Guid.TryParse(Id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + Id + "\"");
            }
            LiveMatch liveMatch = new LiveMatch
            {
                Id = guid,
                MatchId = Guid.Parse(MatchId),
                Score1 = Score1,
                Score2 = Score2,
                State = MatchState.GetMatchState(State),
                TimeStamp = DateTime.Parse(TimeStamp)
            };
            return liveMatch;
        }

        public static LiveMatchDto Create(LiveMatch liveMatch)
        {
            if (liveMatch == null)
            {
                return null;
            }

            LiveMatchDto liveMatchDto = new LiveMatchDto
            {
                Id = liveMatch.Id.ToString(),
                MatchId = liveMatch.MatchId.ToString(),
                Score1 = liveMatch.Score1,
                Score2 = liveMatch.Score2,
                State = liveMatch.State.Name,
                TimeStamp = liveMatch.TimeStamp.ToString(CultureInfo.InvariantCulture)
            };
            return liveMatchDto;
        }
    }
}
