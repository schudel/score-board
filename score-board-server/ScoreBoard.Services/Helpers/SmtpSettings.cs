namespace ScoreBoard.Services.Helpers
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SenderEmailAddress { get; set; }
        public string BaseUrl { get; set; }
        public string LiveMatchUrl { get; set; }
        public string PasswordResetUrl { get; set; }
        public bool UseSsl { get; set; } = true;
        public int Port { get; set; } = 587;
    }
}
