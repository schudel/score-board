using System;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.Infrastructure.Models
{
    public class PlayerEntity
    {
        public virtual Guid Id { get; set; }
        public virtual string PlayerName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual byte[] PasswordHash { get; set; }
        public virtual byte[] PasswordSalt { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool MustChangePassword { get; set; }
        public virtual PlayerRoleEnum Role { get; set; }
        public virtual DateTime? LastLogin { get; set; }
        public virtual string Image { get; set; }
        public virtual DateTime? RegistrationDate { get; set; }
        public virtual DateTime? ActivationDate { get; set; }
        public virtual SettingsEntity Settings { get; set; }

        public virtual Player GetPlayer(bool slim = false)
        {
            Player player = new Player {Id = Id, PlayerName = PlayerName};
            if (slim)
            {
                return player;
            }
            player.FirstName = FirstName;
            player.LastName = LastName;
            player.Email = Email;
            player.PasswordHash = PasswordHash;
            player.PasswordSalt = PasswordSalt;
            player.IsActive = IsActive;
            player.MustChangePassword = MustChangePassword;
            player.Role = PlayerRole.GetPlayerRole(Role);
            player.LastLogin = LastLogin;
            player.Image = Image;
            player.RegistrationDate = RegistrationDate;
            player.ActivationDate = ActivationDate;
            if (Settings != null)
            {
                player.Settings = Settings.GetSettings();
            }
            if (PasswordHash == null)
            {
                PasswordHash = new byte[0];
            }
            if (PasswordSalt == null)
            {
                PasswordSalt = new byte[0];
            }
            return player;
        }

        public static PlayerEntity Create(Player player)
        {
            if (player == null)
            {
                return null;
            }

            PlayerEntity playerEntity = new PlayerEntity
            {
                Id = player.Id,
                PlayerName = player.PlayerName,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Email = player.Email,
                PasswordHash = player.PasswordHash,
                PasswordSalt = player.PasswordSalt,
                IsActive = player.IsActive,
                MustChangePassword = player.MustChangePassword,
                LastLogin = player.LastLogin,
                Image = player.Image,
                RegistrationDate = player.RegistrationDate,
                ActivationDate = player.ActivationDate
            };
            if (player.Settings != null)
            {
                playerEntity.Settings = SettingsEntity.Create(player.Settings);
            }
            if (player.Role != null)
            {
                playerEntity.Role = player.Role.PlayerRoleEnum;
            }
            if (playerEntity.PasswordHash == null)
            {
                playerEntity.PasswordHash = new byte[0];
            }
            if (playerEntity.PasswordSalt == null)
            {
                playerEntity.PasswordSalt = new byte[0];
            }
            return playerEntity;
        }
    }
}
