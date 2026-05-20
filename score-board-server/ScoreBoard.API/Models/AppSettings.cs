namespace ScoreBoard.API.Models
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string DbConnectionString { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
