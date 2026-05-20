using System;
using System.Threading.Tasks;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.Helpers;

namespace ScoreBoard.Services.UseCases
{
    public class PasswordResetRequestService : IPasswordResetRequestService
    {
        private readonly IPasswordResetRequestRepository passwordResetRequestRepository;
        private readonly IEmailService emailService;

        public PasswordResetRequestService(IPasswordResetRequestRepository passwordResetRequestRepository, IEmailService emailService)
        {
            this.passwordResetRequestRepository = passwordResetRequestRepository;
            this.emailService = emailService;
        }

        public void Initialize(string dbConnectionString)
        {
            passwordResetRequestRepository.Initialize(dbConnectionString);
        }

        public async Task<PasswordResetRequest> GetById(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await passwordResetRequestRepository.GetById(guid).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task Add(PasswordResetRequest passwordResetRequest)
        {
            if (passwordResetRequest == null)
            {
                throw new Exception("Password Reset Request is required.");
            }
            await passwordResetRequestRepository.Add(passwordResetRequest).ConfigureAwait(false);
        }

        public async Task Remove(PasswordResetRequest passwordResetRequest)
        {
            if (passwordResetRequest == null)
            {
                throw new Exception("Password Reset Request is required.");
            }
            await passwordResetRequestRepository.Remove(passwordResetRequest).ConfigureAwait(false);
        }

        public async Task Remove(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            PasswordResetRequest passwordResetRequest = await passwordResetRequestRepository.GetById(guid).ConfigureAwait(false);
            await passwordResetRequestRepository.Remove(passwordResetRequest).ConfigureAwait(false);
        }

        public async Task<bool> SendPasswordResetEmail(PasswordResetRequest passwordResetRequest)
        {
            if (passwordResetRequest == null)
            {
                throw new Exception("Password Reset Request is required.");
            }
            // send password reset email
            return await emailService.SendPasswordResetEmail(passwordResetRequest).ConfigureAwait(false);
        }
    }
}
