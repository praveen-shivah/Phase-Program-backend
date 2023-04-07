namespace AuthenticationDto
{
    public class AuthenticateResponseDto
    {
        public bool IsAuthenticated { get; set; }

        public string AccessToken { get; set; } = string.Empty;

        public string Claims { get; set; } = string.Empty;
    }
}
