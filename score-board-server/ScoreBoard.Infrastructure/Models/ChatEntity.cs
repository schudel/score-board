using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class ChatEntity
    {
        public virtual Guid Id { get; set; }
        public virtual Guid PlayerId { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Message { get; set; }
        public virtual string Room { get; set; }
        public virtual DateTime TimeStamp { get; set; }

        public virtual Chat GetChat()
        {
            Chat chat = new Chat
            {
                Id = Id,
                PlayerId = PlayerId,
                UserName = UserName,
                Message = Message,
                Room = Room,
                TimeStamp = TimeStamp
            };
            return chat;
        }

        public static ChatEntity Create(Chat chat)
        {
            if (chat == null)
            {
                return null;
            }
            ChatEntity chatEntity = new ChatEntity
            {
                Id = chat.Id,
                PlayerId = chat.PlayerId,
                UserName = chat.UserName,
                Message = chat.Message,
                Room = chat.Room,
                TimeStamp = chat.TimeStamp
            };
            return chatEntity;
        }
    }
}
