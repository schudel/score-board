using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class TeamEntity
    {
        public virtual Guid Id { get; set; }
        public virtual PlayerEntity Player1 { get; set; }
        public virtual PlayerEntity Player2 { get; set; }
        public virtual string Name { get; set; }

        public virtual Team GetTeam(bool slim = false)
        {
            Team team = new Team
            {
                Id = Id,
                Name = Name
            };
            if (Player1 != null)
            {
                team.Player1 = Player1.GetPlayer(slim);
            }
            if (Player2 != null)
            {
                team.Player2 = Player2.GetPlayer(slim);
            }
            return team;
        }

        public static TeamEntity Create(Team team)
        {
            if (team == null)
            {
                return null;
            }

            TeamEntity teamEntity = new TeamEntity
            {
                Id = team.Id,
                Name = team.Name
            };
            if (team.Player1 != null)
            {
                teamEntity.Player1 = PlayerEntity.Create(team.Player1);
            }
            if (team.Player2 != null)
            {
                teamEntity.Player2 = PlayerEntity.Create(team.Player2);
            }
            return teamEntity;
        }
    }
}
