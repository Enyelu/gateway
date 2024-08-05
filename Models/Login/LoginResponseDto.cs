namespace gateway.api.Models.Login
{
    public class LoginResponseDto
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
