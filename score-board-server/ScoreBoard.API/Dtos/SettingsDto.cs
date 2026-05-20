using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class SettingsDto
    {
        public string Id { get; set; }
        public string LanguageCode { get; set; }
        public bool DarkMode { get; set; }
        public string DashboardLayout { get; set; }

        public Settings GetSettings()
        {
            if (Guid.TryParse(Id, out Guid guid))
            {
                return new Settings
                {
                    Id = guid,
                    LanguageCode = LanguageCode,
                    DarkMode = DarkMode,
                    DashboardLayout = DashboardLayout
                };
            }
            throw new Exception("No valid Id: \"" + Id + "\"");
        }

        public static SettingsDto Create(Settings settings)
        {
            if (settings == null)
            {
                return null;
            }
            SettingsDto settingsDto = new SettingsDto
            {
                Id = settings.Id.ToString(), LanguageCode = settings.LanguageCode, DarkMode = settings.DarkMode, DashboardLayout = settings.DashboardLayout
            };
            return settingsDto;
        }
    }
}
