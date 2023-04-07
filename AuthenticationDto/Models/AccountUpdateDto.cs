namespace AuthenticationDto
{
    public class AccountUpdateDto : BaseDto
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Claims { get; set; } = string.Empty;
    }
}