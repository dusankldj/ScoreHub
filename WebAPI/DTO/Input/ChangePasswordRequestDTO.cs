using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO.Input
{
    public class ChangePasswordRequestDTO
    {
        [Required]
        [MinLength(8)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
