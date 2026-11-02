namespace MAUI.DTO.Output
{
    public class UpdateUsernameRequestDTO
    {
        public string Username { get; set; } = string.Empty;
    }

    public class RequestEmailChangeDTO
    {
        public string NewEmail { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
    }

    public class ConfirmEmailCodeDTO
    {
        public string Code { get; set; } = string.Empty;
    }
}
