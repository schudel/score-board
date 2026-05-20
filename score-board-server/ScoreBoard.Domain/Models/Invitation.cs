using System;
using System.Collections.Generic;

namespace ScoreBoard.Domain.Models
{
    public class Invitation
    {
        public Guid SenderId { get; set; }
        public IList<Guid> ReceiverIdList { get; set; }
        public string MatchId { get; set; }
    }
}
