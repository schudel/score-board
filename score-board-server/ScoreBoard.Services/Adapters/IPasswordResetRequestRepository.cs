using System;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Adapters
{
    public interface IPasswordResetRequestRepository : IBaseRepository
    {
        Task<PasswordResetRequest> GetById(Guid id);
        Task Add(PasswordResetRequest passwordResetRequest);
        Task Remove(PasswordResetRequest passwordResetRequest);
    }
}
