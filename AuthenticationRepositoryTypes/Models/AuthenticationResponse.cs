namespace AuthenticationRepositoryTypes
{
    using APISupportTypes;

    public class AuthenticationResponse
    {
        public int OrganizationId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public bool IsAuthenticated { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public RefreshTokenDataResponse? RefreshToken { get; set; }
        public string Claims { get; set; } = string.Empty;
        public ResponseTypeEnum ResponseType { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

    }
}
