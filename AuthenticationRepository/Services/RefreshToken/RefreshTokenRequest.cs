namespace AuthenticationRepository
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
    }
}
