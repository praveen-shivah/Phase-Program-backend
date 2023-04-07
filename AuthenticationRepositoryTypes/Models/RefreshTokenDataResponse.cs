namespace AuthenticationRepositoryTypes
{
    public enum RefreshTokenResponseType
    {
        successful,

        currentTokenNotFound,

        notFound,

        attemptedReuse,

        expired,

        notActive,

        duplicated
    }

    public class RefreshTokenDataResponse
    {
        public RefreshTokenResponseType ResponseType { get; set; }

        public string? RefreshToken { get; set; } = string.Empty;

        public DateTime? Expires { get; set; }
    }
}
