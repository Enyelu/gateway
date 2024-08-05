namespace gateway.api.Models.Token
{
    public class RefreshTokenToReturnDto
    {
        public string NewJwtAccessToken { get; set; }
        public Guid NewRefreshToken { get; set; }
    }
}
