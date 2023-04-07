namespace AuthenticationRepositoryTypes
{
    public class AuthenticationRequest
    {
        public AuthenticationRequest(
            int organizationId,
            string userId,
            string password,
            string audience,
            string issuer,
            string ipAddress)
        {
            this.OrganizationId = organizationId;
            this.UserId = userId;
            this.Password = password;
            this.Audience = audience;
            this.Issuer = issuer;
            this.IpAddress = ipAddress;
        }

        public string IpAddress { get; }

        public int OrganizationId { get; }

        public string Password { get; }

        public string Audience { get; }

        public string Issuer { get; }

        public string UserId { get; }
    }
}