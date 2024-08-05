namespace gateway.api.Models.Token
{
    public class RefreshTokenRequestDto
    {
        public string UserId { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
