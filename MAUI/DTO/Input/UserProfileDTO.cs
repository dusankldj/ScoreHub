namespace MAUI.DTO.Input
{
    public class UserProfileDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Username { get; set; } 
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
