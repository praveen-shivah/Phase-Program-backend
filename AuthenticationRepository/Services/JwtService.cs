namespace AuthenticationRepository
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using CommonServices;

    using DatabaseContext;

    using Microsoft.IdentityModel.Tokens;

    using SecurityUtilityTypes;

    public class JwtService : IJwtService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly ISecretKeyRetrieval secretKeyRetrieval;

        public JwtService(
            ISecretKeyRetrieval secretKeyRetrieval,
            IDateTimeService dateTimeService)
        {
            this.secretKeyRetrieval = secretKeyRetrieval;
            this.dateTimeService = dateTimeService;
        }

        string IJwtService.GenerateJwtToken(Account account, string issuer, string audience)
        {
            // generate token that is valid for 15 minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.secretKeyRetrieval.GetKey());

            var claims = new List<Claim>();
            claims.Add(new Claim("UserId", account.Id.ToString()));
            claims.Add(new Claim("UserName", account.UserName));
            claims.Add(new Claim("OrganizationId", account.OrganizationId.ToString()));
            var claimsInfo = account.Claims.Split(',');
            for (int x = 0; x < claimsInfo.Length; x = x + 2)
            {
                claims.Add(new Claim(claimsInfo[x], claimsInfo[x + 1]));
            }

            var claimsIdentity = new ClaimsIdentity(claims);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(this.secretKeyRetrieval.GetJwtTokenTTLInMinutes()),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = audience,
                Issuer = issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}