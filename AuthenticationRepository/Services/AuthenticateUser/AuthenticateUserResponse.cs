namespace AuthenticationRepository
{
    using APISupportTypes;

    using DatabaseContext;

    public class AuthenticateUserResponse
    {
        public bool IsSuccessful { get; set; }
        public Account? Account { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public string Claims { get; set; } = string.Empty;
        public RefreshToken? RefreshToken { get; set; }
        public ResponseTypeEnum ResponseType { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
