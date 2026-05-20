using System;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class SettingsEntity
    {
        public virtual Guid Id { get; set; }
        public virtual string LanguageCode { get; set; }
        public virtual bool DarkMode { get; set; }
        public virtual string DashboardLayout { get; set; }


        public virtual Settings GetSettings()
        {
            Settings settings = new Settings
            {
                Id = Id,
                LanguageCode = LanguageCode,
                DarkMode = DarkMode,
                DashboardLayout = DashboardLayout
            };
            return settings;
        }

        public static SettingsEntity Create(Settings settings)
        {
            if (settings == null)
            {
                return null;
            }
            SettingsEntity settingsEntity = new SettingsEntity
            {
                Id = settings.Id,
                LanguageCode = settings.LanguageCode,
                DarkMode = settings.DarkMode,
                DashboardLayout = settings.DashboardLayout
            };
            return settingsEntity;
        }
    }
}
