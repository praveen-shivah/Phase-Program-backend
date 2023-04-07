namespace AuthenticationDto
{
    using System.ComponentModel;

    public class AuthenticateRequestDto
    {
        [DefaultValue(1)]
        public int OrganizationId { get; set; }

        [DefaultValue("test")]
        public string ApiKey { get; set; } = string.Empty;

        [DefaultValue("carl")]
        public string User { get; set; } = string.Empty;

        [DefaultValue("1234")]
        public string Password { get; set; } = string.Empty;

        [DefaultValue("")]
        public string Audience { get; set; } = string.Empty;

        [DefaultValue("")]
        public string Issuer { get; set; } = string.Empty;
    }
}
