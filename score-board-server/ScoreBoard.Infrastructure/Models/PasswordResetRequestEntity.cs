using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class PasswordResetRequestEntity
    {
        public virtual Guid Id { get; set; }
        public virtual Guid PlayerId { get; set; }
        public virtual DateTime TimeStamp { get; set; }

        public virtual PasswordResetRequest GetPasswordResetRequest()
        {
            PasswordResetRequest passwordResetRequest = new PasswordResetRequest
            {
                Id = Id,
                Player = new Player { Id = PlayerId },
                TimeStamp = TimeStamp
            };
            return passwordResetRequest;
        }

        public static PasswordResetRequestEntity Create(PasswordResetRequest passwordResetRequest)
        {
            if (passwordResetRequest == null)
            {
                return null;
            }
            PasswordResetRequestEntity passwordResetRequestEntity = new PasswordResetRequestEntity
            {
                Id = passwordResetRequest.Id,
                PlayerId = passwordResetRequest.Player.Id,
                TimeStamp = passwordResetRequest.TimeStamp
            };
            return passwordResetRequestEntity;
        }
    }
}
