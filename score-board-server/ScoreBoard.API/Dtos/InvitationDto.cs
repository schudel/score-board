using System;
using System.Collections.Generic;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class InvitationDto
    {
        public string SenderId { get; set; }
        public IList<string> ReceiverIdList { get; set; }
        public string MatchId { get; set; }

        public Invitation GetInvitation()
        {
            Invitation invitation = new Invitation();
            if (!Guid.TryParse(SenderId, out Guid senderGuid))
            {
                throw new Exception("Invalid Sender Id");
            }
            invitation.SenderId = senderGuid;
            IList<Guid> receiverList = new List<Guid>();
            foreach (string receiverId in ReceiverIdList)
            {
                if (!Guid.TryParse(receiverId, out Guid receiverGuid))
                {
                    throw new Exception("Invalid Receiver Id");
                }
                receiverList.Add(receiverGuid);
            }
            invitation.ReceiverIdList = receiverList;
            invitation.MatchId = MatchId;
            return invitation;
        }
    }
}
