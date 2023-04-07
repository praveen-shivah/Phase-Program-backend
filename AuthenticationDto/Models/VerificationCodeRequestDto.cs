namespace AuthenticationDto
{
    public class VerificationCodeRequestDto
    {
        public string VerificationCode { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
