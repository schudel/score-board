using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.Helpers;

namespace ScoreBoard.Services.UseCases
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository playerRepository;
        private readonly IPasswordService passwordService;
        private readonly IEmailService emailService;
        private readonly ISettingsService settingsService;
        private readonly IRatingService ratingService;

        public PlayerService(IPlayerRepository playerRepository, IPasswordService passwordService, IEmailService emailService, ISettingsService settingsService, IRatingService ratingService)
        {
            this.playerRepository = playerRepository;
            this.passwordService = passwordService;
            this.emailService = emailService;
            this.settingsService = settingsService;
            this.ratingService = ratingService;
        }

        public void Initialize(string dbConnectionString)
        {
            playerRepository.Initialize(dbConnectionString);
            settingsService.Initialize(dbConnectionString);
            ratingService.Initialize(dbConnectionString);
        }

        public async Task<Player> Authenticate(string playerName, string password)
        {
            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Username or password is empty.");
            }

            Player player = await playerRepository.GetPlayerByPlayerName(playerName).ConfigureAwait(false);
            // check if player exists
            if (player == null)
            {
                throw new Exception("Username or password is incorrect.");
            }
            // check if password is correct
            if (!passwordService.VerifyPasswordHash(password, player.PasswordHash, player.PasswordSalt))
            {
                throw new Exception("Username or password is incorrect.");
            }
            // check if player is active
            if (!player.IsActive)
            {
                throw new Exception("Your account is inactive. Please activate your account via the email you just received from us.");
            }
            // authentication successful --> update last login
            player.LastLogin = DateTime.Now;
            await playerRepository.Update(player).ConfigureAwait(false);
            return player;
        }

        public async Task<bool> Register(Player player, string password)
        {
            player.Role = PlayerRole.User;
            player.RegistrationDate = DateTime.Now;
            player = await Create(player, password);
            // send activation email
            return await emailService.SendActivationEmail(player).ConfigureAwait(false);
        }

        public async Task<bool> Activate(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Player player = await playerRepository.GetById(guid);
            if (player == null)
            {
                return false;
            }
            player.IsActive = true;
            player.ActivationDate = DateTime.Now;
            await playerRepository.Update(player).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> ResendEmail(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Player player = await playerRepository.GetById(guid).ConfigureAwait(false);
            if (player == null)
            {
                return false;
            }
            return await emailService.ResendActivationEmail(player).ConfigureAwait(false);
        }

        public async Task<Player> GetById(string id, bool slim = false)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                return await playerRepository.GetById(guid, slim).ConfigureAwait(false);
            }
            throw new Exception("No valid Id: \"" + id + "\"");
        }

        public async Task<ICollection<Player>> GetAll() => await playerRepository.GetAll().ConfigureAwait(false);

        public async Task<Player> Create(Player player, string password, bool isActive = false)
        {
            // validation
            if (player == null)
            {
                throw new Exception("Player is required.");
            }
            if (string.IsNullOrEmpty(player.PlayerName))
            {
                throw new Exception("PlayerName is required.");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password is required.");
            }
            if (string.IsNullOrEmpty(player.Email))
            {
                throw new Exception("Email is required.");
            }
            if (!IsValidEmail(player.Email))
            {
                throw new Exception("Email Address is not valid.");
            }
            if (await playerRepository.GetPlayerByPlayerName(player.PlayerName).ConfigureAwait(false) != null)
            {
                throw new Exception("PlayerName \"" + player.PlayerName + "\" is already taken.");
            }
            if (await playerRepository.GetByEmail(player.Email).ConfigureAwait(false) != null)
            {
                throw new Exception("Email \"" + player.Email + "\" is already taken.");
            }
            // set common values
            player.IsActive = isActive;
            if (isActive)
            {
                player.RegistrationDate = DateTime.Now;
                player.ActivationDate = DateTime.Now;
            }
            player.MustChangePassword = false;
            if (player.Role == null)
            {
                player.Role = PlayerRole.User;
            }

            // create password hash and salt
            passwordService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            player.PasswordHash = passwordHash;
            player.PasswordSalt = passwordSalt;
            // create default settings
            player.Settings = SettingsService.CreateDefaultSettings();

            // add player to database
            await playerRepository.Add(player).ConfigureAwait(false);
            return player;
        }

        public async Task Update(string id, Player player)
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

            if (player.PlayerName != p.PlayerName)
            {
                // PlayerName has changed so check if the new username is already taken
                if (await playerRepository.GetPlayerByPlayerName(player.PlayerName).ConfigureAwait(false) != null)
                {
                    throw new Exception("PlayerName \"" + player.PlayerName + "\" is already taken");
                }
            }

            // update properties
            p.PlayerName = player.PlayerName;
            p.FirstName = player.FirstName;
            p.LastName = player.LastName;
            p.Email = player.Email;
            p.Role = player.Role;
            p.MustChangePassword = player.MustChangePassword;
            p.IsActive = player.IsActive;
            p.LastLogin = player.LastLogin;
            p.RegistrationDate = player.RegistrationDate;
            p.ActivationDate = player.ActivationDate;
            p.Image = player.Image;
            
            await playerRepository.Update(p).ConfigureAwait(false);
        }

        public async Task Remove(Player player)
        {
            if (player == null)
            {
                throw new Exception("Player is required.");
            }
            await playerRepository.Remove(player).ConfigureAwait(false);
        }

        public async Task Remove(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Player player = await playerRepository.GetById(guid).ConfigureAwait(false);
            await playerRepository.Remove(player).ConfigureAwait(false);
        }

        public async Task<long> Count() => await playerRepository.Count().ConfigureAwait(false);

        public async Task ChangePassword(string id, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new Exception("New Password is required");
            }
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Player p = await playerRepository.GetById(guid).ConfigureAwait(false);
            if (p == null)
            {
                throw new Exception("Player not found");
            }
            if (!passwordService.VerifyPasswordHash(currentPassword, p.PasswordHash, p.PasswordSalt))
            {
                throw new Exception("The current Password is not correct");
            }

            // update password if it was entered
            passwordService.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            p.PasswordHash = passwordHash;
            p.PasswordSalt = passwordSalt;
            await playerRepository.Update(p).ConfigureAwait(false);
        }

        public async Task ResetPassword(string id, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new Exception("New Password is required");
            }
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("No valid Id: \"" + id + "\"");
            }
            Player p = await playerRepository.GetById(guid).ConfigureAwait(false);
            if (p == null)
            {
                throw new Exception("Player not found");
            }

            // update password if it was entered
            passwordService.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            p.PasswordHash = passwordHash;
            p.PasswordSalt = passwordSalt;
            await playerRepository.Update(p).ConfigureAwait(false);
        }

        public async Task<Player> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("E-Mail Address is required");
            }
            return await playerRepository.GetByEmail(email).ConfigureAwait(false);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress address = new MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
