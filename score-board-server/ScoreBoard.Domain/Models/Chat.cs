using System;

namespace ScoreBoard.Domain.Models
{
    public class Chat
    {
        public Chat()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
        }

        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string Room { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
