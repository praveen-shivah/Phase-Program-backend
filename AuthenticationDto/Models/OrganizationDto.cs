namespace AuthenticationDto
{
    public class OrganizationDto : BaseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string APIKey { get; set; } = string.Empty;

        public string URL { get; set; } = string.Empty;
    }
}