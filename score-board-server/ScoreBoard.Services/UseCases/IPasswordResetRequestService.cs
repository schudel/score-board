using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.UseCases
{
    public interface IPasswordResetRequestService
    {
        void Initialize(string dbConnectionString);
        Task<PasswordResetRequest> GetById(string id);
        Task Add(PasswordResetRequest passwordResetRequest);
        Task Remove(PasswordResetRequest passwordResetRequest);
        Task Remove(string id);
        Task<bool> SendPasswordResetEmail(PasswordResetRequest passwordResetRequest);
    }
}
