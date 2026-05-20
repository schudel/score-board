using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface ISettingsService
    {
        void Initialize(string dbConnectionString);
        Task Update(string id, Settings settings);
    }
}
