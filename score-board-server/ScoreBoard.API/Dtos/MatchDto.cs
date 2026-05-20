using System;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class MatchDto
    {
        public string Id { get; set; }

        //[Required]
        public GameDto Game { get; set; }

        //[Required]
        public TeamDto Team1 { get; set; }

        //[Required]
        public TeamDto Team2 { get; set; }

        public string StartTime { get; set; }

        public string StopTime { get; set; }

        public string Duration { get; set; }

        //[Required]
        public int Score1 { get; set; }

        //[Required]
        public int Score2 { get; set; }

        public string State { get; set; }

        public double? MatchQuality { get; set; }

        public bool? Drawn { get; set; }

        public string WinnerTeamId { get; set; }

        public string LoserTeamId { get; set; }

        public Match GetMatch()
        {
            if (!Guid.TryParse(Id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + Id + "\"");
            }

            Match match = new Match
            {
                Id = guid,
                Game = Game.GetGame(),
                Score1 = Score1,
                Score2 = Score2,
                State = MatchState.GetMatchState(State),
                MatchQuality = MatchQuality,
                StartTime = DateTime.TryParse(StartTime, out DateTime startTime) ? (DateTime?) startTime : null,
                StopTime = DateTime.TryParse(StopTime, out DateTime stopTime) ? (DateTime?) stopTime : null
            };
            if (Team1 != null)
            {
                match.Team1 = Team1.GetTeam();
            }
            if (Team2 != null)
            {
                match.Team2 = Team2.GetTeam();
            }
            return match;
        }

        public static MatchDto Create(Match match)
        {
            if (match == null)
            {
                return null;
            }

            MatchDto matchDto = new MatchDto
            {
                Id = match.Id.ToString(),
                Game = GameDto.Create(match.Game),
                StartTime = match.StartTime.ToString(),
                StopTime = match.StopTime.ToString(),
                Score1 = match.Score1,
                Score2 = match.Score2,
                Drawn = match.Drawn,
                State = match.State.Name,
                MatchQuality = match.MatchQuality,
                WinnerTeamId = match.Winner?.Id.ToString(),
                LoserTeamId = match.Loser?.Id.ToString(),
                Duration = match.Duration.ToString()
            };
            if (match.Team1 != null)
            {
                matchDto.Team1 = TeamDto.Create(match.Team1);
            }
            if (match.Team2 != null)
            {
                matchDto.Team2 = TeamDto.Create(match.Team2);
            }
            return matchDto;
        }
    }
}
