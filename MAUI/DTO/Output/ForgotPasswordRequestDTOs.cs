namespace MAUI.DTO.Output
{
    public class ForgotPasswordRequestDTO
    {
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyResetCodeRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class ResetPasswordRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
