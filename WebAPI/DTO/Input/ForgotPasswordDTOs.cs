using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO.Input
{
    public class ForgotPasswordRequestDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyResetCodeDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;
    }

    public class ResetPasswordRequestDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string ResetToken { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
