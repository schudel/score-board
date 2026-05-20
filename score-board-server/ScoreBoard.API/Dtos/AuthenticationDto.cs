using System.ComponentModel.DataAnnotations;

namespace ScoreBoard.API.Dtos
{
    public class AuthenticationDto
    {
        [Required]
        public string PlayerName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
