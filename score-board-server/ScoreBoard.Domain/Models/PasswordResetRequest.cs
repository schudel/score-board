using System;

namespace ScoreBoard.Domain.Models
{
    public class PasswordResetRequest
    {
        public PasswordResetRequest()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Player Player { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
