using System;
using PlayerRole = ScoreBoard.Domain.Enums.PlayerRole;

namespace ScoreBoard.Domain.Models
{
    public class Player
    {
        public Player()
        {
            Id = Guid.NewGuid();
            Role = PlayerRole.Guest;
        }

        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }
        public string Image { get; set; }
        public PlayerRole Role { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public Settings Settings { get; set; }
    }
}
