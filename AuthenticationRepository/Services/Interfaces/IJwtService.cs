namespace AuthenticationRepository
{
    using DatabaseContext;

    public interface IJwtService
    {
        public string GenerateJwtToken(Account account, string issuer, string audience);
    }
}