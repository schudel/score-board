namespace ScoreBoard.API.Dtos
{
    public class ChatTypingStateDto
    {
        public string PlayerId { get; set; }
        public string UserName { get; set; }
        public bool IsTyping { get; set; }
        public string Room { get; set; }
        public string TimeStamp { get; set; }
    }
}
