using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO.Input
{
    public class UpdateUsernameRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;
    }

    public class RequestEmailChangeDTO
    {
        [Required]
        public string NewEmail { get; set; } = string.Empty;

        // Ponovna provera identiteta - e-mail sluzi za oporavak naloga, pa se menja samo uz lozinku.
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
    }

    public class ConfirmEmailCodeDTO
    {
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
