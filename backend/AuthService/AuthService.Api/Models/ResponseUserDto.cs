namespace AuthService.Api.Models
{
    public class ResponseUserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}