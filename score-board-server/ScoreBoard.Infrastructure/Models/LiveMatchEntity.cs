using System;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class LiveMatchEntity
    {
        public virtual Guid Id { get; set; }
        public virtual Guid MatchId { get; set; }
        public virtual int Score1 { get; set; }
        public virtual int Score2 { get; set; }
        public virtual MatchStateEnum State { get; set; }
        public virtual DateTime TimeStamp { get; set; }

        public virtual LiveMatch GetLiveMatch(bool slim = false)
        {
            LiveMatch liveMatch = new LiveMatch
            {
                Id = Id,
                MatchId = MatchId,
                Score1 = Score1,
                Score2 = Score2,
                State = MatchState.GetMatchState(State),
                TimeStamp = TimeStamp
            };
            return liveMatch;
        }

        public static LiveMatchEntity Create(LiveMatch liveMatch)
        {
            if (liveMatch == null)
            {
                return null;
            }
            LiveMatchEntity liveMatchEntity = new LiveMatchEntity
            {
                Id = liveMatch.Id,
                MatchId = liveMatch.MatchId,
                Score1 = liveMatch.Score1,
                Score2 = liveMatch.Score2,
                State = liveMatch.State.MatchStateEnum,
                TimeStamp = liveMatch.TimeStamp
            };
            return liveMatchEntity;
        }
    }
}
