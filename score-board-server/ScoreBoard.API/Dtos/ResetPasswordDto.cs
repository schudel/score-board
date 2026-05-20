using System.ComponentModel.DataAnnotations;

namespace ScoreBoard.API.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string PasswordRequestId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
