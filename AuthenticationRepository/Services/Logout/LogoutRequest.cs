namespace AuthenticationRepository
{
    public class LogoutRequest
    {
        public LogoutRequest(int organizationId, string userName, string issuer, string audience)
        {
            this.OrganizationId = organizationId;
            this.UserName = userName;
            this.Issuer = issuer;
            this.Audience = audience;
        }

        public int OrganizationId { get; }

        public string UserName { get; }

        public string Issuer { get; }

        public string Audience { get; }
    }
}