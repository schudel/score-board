using System;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;

namespace ScoreBoard.API.Dtos
{
    public class PlayerDto
    {
        public string Id { get; set; }

        //[Required]
        public string PlayerName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        //[Required]
        public string Email { get; set; }

        //[Required]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        public bool MustChangePassword { get; set; }

        public string RoleName { get; set; }

        public string Token { get; set; }

        public string LastLogin { get; set; }

        public string Image { get; set; }

        public string RegistrationDate { get; set; }

        public string ActivationDate { get; set; }

        public SettingsDto Settings { get; set; }

        public Player GetPlayer()
        {
            if (!Guid.TryParse(Id, out Guid guid))
            {
                guid = Guid.NewGuid();
            }
            Player player = new Player
            {
                Id = guid,
                PlayerName = PlayerName,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                IsActive = IsActive,
                MustChangePassword = MustChangePassword,
                Role = string.IsNullOrEmpty(RoleName) ? PlayerRole.User : PlayerRole.GetPlayerRole(RoleName),
                LastLogin = string.IsNullOrEmpty(LastLogin) ? (DateTime?) null : DateTime.Parse(LastLogin),
                Image = Image ?? "",
                RegistrationDate = string.IsNullOrEmpty(RegistrationDate) ? (DateTime?)null : DateTime.Parse(RegistrationDate),
                ActivationDate = string.IsNullOrEmpty(ActivationDate) ? (DateTime?)null : DateTime.Parse(ActivationDate)
            };
            if (Settings != null)
            {
                player.Settings = Settings.GetSettings();
            }
            return player;
        }

        public static PlayerDto Create(Player player, string token = "")
        {
            if (player == null)
            {
                return null;
            }
            PlayerDto playerDto = new PlayerDto
            {
                Id = player.Id.ToString(),
                Token = token,
                PlayerName = player.PlayerName,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Email = player.Email,
                IsActive = player.IsActive,
                MustChangePassword = player.MustChangePassword,
                RoleName = player.Role.Name,
                LastLogin = player.LastLogin.ToString(),
                Image = player.Image,
                RegistrationDate = player.RegistrationDate.ToString(),
                ActivationDate = player.ActivationDate.ToString(),
                Settings = SettingsDto.Create(player.Settings)
            };
            return playerDto;
        }
    }
}
