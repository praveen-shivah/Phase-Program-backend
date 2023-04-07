namespace AuthenticationRepository
{
    /// <summary>
    /// AuthenticateUserRequest
    /// Audience is the entity receiving the token (i.e. sessionid in a browser or terminal id)
    /// Issuer is the service requesting the token (i.e. API identifier)
    /// </summary>
    public class AuthenticateUserRequest
    {
        public int OrganizationId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string IpAddress { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;
    }
}
