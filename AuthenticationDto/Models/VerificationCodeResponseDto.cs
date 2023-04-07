namespace AuthenticationDto
{
    public class VerificationCodeResponseDto
    {
        public bool IsSuccessful { get; set; }

        public string PinNumber { get; set; } = string.Empty;
    }
}
