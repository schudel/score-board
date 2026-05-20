using System;
using MatchState = ScoreBoard.Domain.Enums.MatchState;

namespace ScoreBoard.Domain.Models
{
    public class Match
    {
        public Match()
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.Now;
            State = MatchState.Scheduled;
            Team1 = new Team();
            Team2 = new Team();
        }

        public Guid Id { get; set; }
        public Game Game { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public MatchState State { get; set; }
        public double? MatchQuality { get; set; }
        // convenience getter 
        public bool Drawn => Score1 == Score2;
        public Team Winner => Score1 > Score2 ? Team1 : Score2 > Score1 ? Team2 : null;
        public Team Loser => Score1 > Score2 ? Team2 : Score2 > Score1 ? Team1 : null;
        public TimeSpan? Duration => StopTime - StartTime;
    }
}
