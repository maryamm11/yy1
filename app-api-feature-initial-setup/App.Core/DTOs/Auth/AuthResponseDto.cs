namespace App.Core.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? UserType { get; set; }
        public Guid? UserId { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
