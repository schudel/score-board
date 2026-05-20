using System;
using System.Globalization;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class ChatDto
    {
        public string Id { get; set; }
        public string PlayerId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string Room { get; set; }
        public string TimeStamp { get; set; }

        public Chat GetChat()
        {
            if (!Guid.TryParse(Id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + Id + "\"");
            }
            Chat chat = new Chat
            {
                Id = guid,
                PlayerId = Guid.Parse(PlayerId),
                UserName = UserName,
                Message = Message,
                Room = Room,
                TimeStamp = DateTime.Parse(TimeStamp)
            };
            return chat;
        }

        public static ChatDto Create(Chat chat)
        {
            if (chat == null)
            {
                return null;
            }

            ChatDto chatDto = new ChatDto
            {
                Id = chat.Id.ToString(),
                PlayerId = chat.PlayerId.ToString(),
                UserName = chat.UserName,
                Message = chat.Message,
                Room = chat.Room,
                TimeStamp = chat.TimeStamp.ToString(CultureInfo.CurrentCulture)
            };
            return chatDto;
        }
    }
}
