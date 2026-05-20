using System;
using MatchState = ScoreBoard.Domain.Enums.MatchState;

namespace ScoreBoard.Domain.Models
{
    public class LiveMatch
    {
        public LiveMatch()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
        }

        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public MatchState State { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
