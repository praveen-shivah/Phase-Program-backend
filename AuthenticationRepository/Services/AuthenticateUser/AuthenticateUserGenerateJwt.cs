namespace AuthenticationRepository
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using CommonServices;
    using DatabaseContext;
    using Microsoft.IdentityModel.Tokens;
    using SecurityUtilityTypes;

    public class AuthenticateUserGenerateJwt : IAuthenticateUser
    {
        private readonly IAuthenticateUser authenticateUser;

        private readonly IJwtService jwtService;

        private readonly ISecretKeyRetrieval secretKeyRetrieval;

        private readonly IDateTimeService dateTimeService;

        public AuthenticateUserGenerateJwt(
            IAuthenticateUser authenticateUser, 
            IJwtService jwtService,
            ISecretKeyRetrieval secretKeyRetrieval, 
            IDateTimeService dateTimeService)
        {
            this.authenticateUser = authenticateUser;
            this.jwtService = jwtService;
            this.secretKeyRetrieval = secretKeyRetrieval;
            this.dateTimeService = dateTimeService;
        }

        async Task<AuthenticateUserResponse> IAuthenticateUser.Authenticate(DataContext context, AuthenticateUserRequest authenticateUserRequest)
        {
            var response = await this.authenticateUser.Authenticate(context, authenticateUserRequest);
            if (!response.IsSuccessful || !response.IsAuthenticated || response.Account == null)
            {
                return response;
            }

            response.JwtToken = this.jwtService.GenerateJwtToken(response.Account, authenticateUserRequest.Issuer, authenticateUserRequest.Audience);

            var refreshToken = new RefreshToken
            {
                Token = getUniqueToken(),
                Expires = this.dateTimeService.UtcNow.AddDays(this.secretKeyRetrieval.GetRefreshTokenTTLInDays()),
                CreatedOn = this.dateTimeService.UtcNow,
                CreatedByIp = authenticateUserRequest.IpAddress.Trim(),
                ReasonRevoked = string.Empty,
                ReplacedByToken = string.Empty,
                Issuer = authenticateUserRequest.Issuer.Trim(),
                Audience = authenticateUserRequest.Audience.Trim(),
                IsActive = true
            };

            response.Account.RefreshToken.Add(refreshToken);
            response.RefreshToken = refreshToken;

            return response;

            string getUniqueToken()
            {
                // token is a cryptographically strong random sequence of values
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                // ensure token is unique by checking against db
                var tokenIsUnique = !context.Account.Any(u => u.RefreshToken.Any(t => t.Token == token));
                if (!tokenIsUnique)
                {
                    return getUniqueToken();
                }

                return token;
            }
        }
    }
}
