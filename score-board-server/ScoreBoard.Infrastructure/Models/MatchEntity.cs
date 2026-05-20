using System;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class MatchEntity
    {
        public virtual Guid Id { get; set; }
        public virtual GameEntity Game { get; set; }
        public virtual TeamEntity Team1 { get; set; }
        public virtual TeamEntity Team2 { get; set; }
        public virtual DateTime? StartTime { get; set; }
        public virtual DateTime? StopTime { get; set; }
        public virtual int Score1 { get; set; }
        public virtual int Score2 { get; set; }
        public virtual MatchStateEnum State { get; set; }
        public virtual double? MatchQuality { get; set; }

        public virtual Match GetMatch(bool slim = false)
        {
            Match match = new Match
            {
                Id = Id,
                StartTime = StartTime,
                StopTime = StopTime,
                Score1 = Score1,
                Score2 = Score2,
                State = MatchState.GetMatchState(State),
                MatchQuality = MatchQuality
            };
            if (Game != null)
            {
                match.Game = Game.GetGame(slim);
            }
            if (Team1 != null)
            {
                match.Team1 = Team1.GetTeam(slim);
            }
            if (Team2 != null)
            {
                match.Team2 = Team2.GetTeam(slim);
            }
            return match;
        }

        public static MatchEntity Create(Match match)
        {
            if (match == null)
            {
                return null;
            }
            MatchEntity matchEntity = new MatchEntity
            {
                Id = match.Id,
                StartTime = match.StartTime,
                StopTime = match.StopTime,
                Score1 = match.Score1,
                Score2 = match.Score2,
                State = match.State.MatchStateEnum,
                MatchQuality = match.MatchQuality
            };
            if (match.Game != null)
            {
                GameEntity gameEntity = GameEntity.Create(match.Game);
                matchEntity.Game = gameEntity;
            }
            if (match.Team1 != null)
            {
                matchEntity.Team1 = TeamEntity.Create(match.Team1);
            }
            if (match.Team2 != null)
            {
                matchEntity.Team2 = TeamEntity.Create(match.Team2);
            }
            return matchEntity;
        }
    }
}
