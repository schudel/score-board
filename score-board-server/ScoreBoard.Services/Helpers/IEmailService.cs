using System.Threading.Tasks;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Helpers
{
    public interface IEmailService
    {
        Task<bool> SendActivationEmail(Player player);
        Task<bool> ResendActivationEmail(Player player);
        Task<bool> SendInvitation(string senderName, string receiverName, string[] receiverEmail, string matchId);
        Task<bool> SendPasswordResetEmail(PasswordResetRequest passwordResetRequest);
    }
}
