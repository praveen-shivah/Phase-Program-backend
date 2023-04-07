namespace AuthenticationRepository
{
    using AuthenticationRepositoryTypes;

    using DatabaseContext;

    public class RefreshTokenResponse
    {
        public bool IsSuccessful { get; set; }

        public RefreshTokenResponseType RefreshTokenResponseType { get; set; }

        public Account? Account { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public RefreshTokenDataResponse? RefreshToken { get; set; }

        public string? JwtToken { get; set; }

        public RefreshToken? CurrentRefreshToken { get; set; }

        public string Audience { get; set; } = string.Empty;

        public string CreatedByIp { get; set; } = string.Empty;
    }
}