using System;

namespace ScoreBoard.Domain.Models
{
    public class Settings
    {
        public Settings()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string LanguageCode { get; set; }
        public bool DarkMode { get; set; }
        public string DashboardLayout { get; set; }
    }
}
