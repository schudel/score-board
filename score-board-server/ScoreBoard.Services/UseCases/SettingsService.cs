using System;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Services.UseCases
{
    public class SettingsService : ISettingsService
    {
        private readonly IPlayerRepository playerRepository;

        public SettingsService(IPlayerRepository playerRepository)
        {
            this.playerRepository = playerRepository;
        }

        public void Initialize(string dbConnectionString)
        {
            playerRepository.Initialize(dbConnectionString);
        }

        public static Settings CreateDefaultSettings()
        {
            Settings settings = new Settings
            {
                LanguageCode = "en-us",
                DarkMode = false,
                DashboardLayout =
                    "[{\"cols\":3,\"rows\":4,\"y\":0,\"x\":3,\"label\":\"ScoreBoardInfo\"},{\"cols\":6,\"rows\":4,\"y\":4,\"x\":0,\"label\":\"LiveMatches\"},{\"cols\":2,\"rows\":2,\"y\":0,\"x\":6,\"label\":\"NoOneWidget\"},{\"cols\":5,\"rows\":4,\"y\":0,\"x\":8,\"label\":\"Ranking\"},{\"cols\":3,\"rows\":4,\"y\":0,\"x\":0,\"label\":\"MyStatistics\"},{\"cols\":7,\"rows\":4,\"y\":4,\"x\":6,\"label\":\"MyMatches\"},{\"cols\":2,\"rows\":2,\"y\":2,\"x\":6,\"label\":\"StartMatch\"}]"
            };
            return settings;
        }

        public async Task Update(string id, Settings settings)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Player p = await playerRepository.GetById(guid).ConfigureAwait(false);
            if (p == null)
            {
                throw new Exception("Player not found");
            }
            p.Settings = settings;
            await playerRepository.Update(p).ConfigureAwait(false);
        }
    }
}
