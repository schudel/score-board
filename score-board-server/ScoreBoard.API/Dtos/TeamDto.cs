using System;
using System.ComponentModel.DataAnnotations;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class TeamDto
    {
        public string Id { get; set; }

        [Required]
        public PlayerDto Player1 { get; set; }

        public PlayerDto Player2 { get; set; }

        public string Name { get; set; }

        public Team GetTeam()
        {
            if (!Guid.TryParse(Id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + Id + "\"");
            }
            Team team = new Team
            {
                Id = guid,
                Name = Name
            };
            if (Player1 != null)
            {
                team.Player1 = Player1.GetPlayer();
            }
            if (Player2 != null)
            {
                team.Player2 = Player2.GetPlayer();
            }
            return team;
        }

        public static TeamDto Create(Team team)
        {
            if (team == null)
            {
                return null;
            }
            TeamDto teamDto = new TeamDto
            {
                Id = team.Id.ToString(),
                Player1 = PlayerDto.Create(team.Player1),
                Player2 = PlayerDto.Create(team.Player2),
                Name = team.Name
            };
            return teamDto;
        }
    }
}
