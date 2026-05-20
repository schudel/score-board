using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Services.Helpers
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings settings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            settings = smtpSettings.Value;
        }

        public async Task<bool> SendActivationEmail(Player player)
        {
            try
            {
                await SendEmail(new[] { player.Email }, "Score-Board Registration",
                    "Hi " + player.PlayerName + "\r\n\r\n" +
                    "We are very happy to welcome you as a new member.\r\n" +
                    "To complete the registration, please click on the following link:\r\n\r\n" +
                    settings.BaseUrl + player.Id + "\r\n\r\n" +
                    "Best regards\r\n\r\nYour Score-Board Team").ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResendActivationEmail(Player player)
        {
            try
            {
                await SendEmail(new[] { player.Email }, "Score-Board Registration - New Attempt",
                    "Hi " + player.PlayerName + "\r\n\r\n" +
                    "We are very happy to welcome you as a new member.\r\n" +
                    "To complete the registration, please click on the following link:\r\n\r\n" +
                    settings.BaseUrl + player.Id + "\r\n\r\n" +
                    "Best regards\r\n\r\nYour Score-Board Team").ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendInvitation(string senderName, string receiverName, string[] receiverEmail, string matchId)
        {
            try
            {
                await SendEmail(receiverEmail, "Score-Board Invitation", 
                          "Hi " + receiverName + "\r\n\r\n" +
                          "You received an invitation from Player: " + senderName + ". \r\n" +
                          "You can find more information about the match under the following link:\r\n" +
                          settings.LiveMatchUrl + matchId + "\r\n\r\n" +
                          "Best regards\r\n\r\nYour Score-Board Team").ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendPasswordResetEmail(PasswordResetRequest passwordResetRequest)
        {
            try
            {
                await SendEmail(new[] { passwordResetRequest.Player.Email }, "Score-Board - Reset Password Request",
                    "Hi " + passwordResetRequest.Player.PlayerName + "\r\n\r\n" +
                    "You can reset your Password under the following link:\r\n" +
                    settings.PasswordResetUrl + passwordResetRequest.Id + "\r\n\r\n" +
                    "Best regards\r\n\r\nYour Score-Board Team").ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private async Task SendEmail(IEnumerable<string> emailAddresses, string subject, string body)
        {
            SmtpClient client = new SmtpClient(settings.Server)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(settings.Username, settings.Password),
                EnableSsl = settings.UseSsl,
                Port = settings.Port
            };

            MailMessage mailMessage = new MailMessage {From = new MailAddress(settings.SenderEmailAddress, settings.SenderEmailAddress)};
            foreach (string emailAddress in emailAddresses)
            {
                mailMessage.To.Add(emailAddress);
            }
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            await client.SendMailAsync(mailMessage).ConfigureAwait(false);
        }
    }
}
