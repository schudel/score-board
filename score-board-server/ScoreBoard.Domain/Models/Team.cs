using System;

namespace ScoreBoard.Domain.Models
{
    public class Team
    {
        public Team()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public string Name { get; set; }
    }
}
