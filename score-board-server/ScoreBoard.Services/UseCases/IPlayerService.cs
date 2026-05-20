using System.Collections.Generic;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface IPlayerService
    {
        void Initialize(string dbConnectionString);
        Task<Player> Authenticate(string playerName, string password);
        Task<bool> Register(Player player, string password);
        Task<bool> Activate(string id);
        Task<bool> ResendEmail(string id);
        Task<Player> GetById(string id, bool slim = false);
        Task<ICollection<Player>> GetAll();
        Task<Player> Create(Player player, string password, bool isActive = false);
        Task Update(string id, Player player);
        Task ChangePassword(string id, string currentPassword, string newPassword);
        Task Remove(Player player);
        Task Remove(string id);
        Task<long> Count();
        Task<Player> GetByEmail(string email);
        Task ResetPassword(string id, string newPassword);
    }
}
