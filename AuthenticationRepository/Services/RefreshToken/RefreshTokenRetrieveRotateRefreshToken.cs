namespace AuthenticationRepository
{
    using System.Security.Cryptography;

    using AuthenticationRepositoryTypes;

    using CommonServices;
    using DatabaseContext;
    using SecurityUtilityTypes;

    public class RefreshTokenRetrieveRotateRefreshToken : IRefreshToken
    {
        private readonly IDateTimeService dateTimeService;

        private readonly IJwtService jwtService;

        private readonly IRefreshToken refreshToken;

        private readonly ISecretKeyRetrieval secretKeyRetrieval;

        public RefreshTokenRetrieveRotateRefreshToken(
            IRefreshToken refreshToken,
            IDateTimeService dateTimeService,
            IJwtService jwtService,
            ISecretKeyRetrieval secretKeyRetrieval)
        {
            this.refreshToken = refreshToken;
            this.dateTimeService = dateTimeService;
            this.jwtService = jwtService;
            this.secretKeyRetrieval = secretKeyRetrieval;
        }

        async Task<RefreshTokenResponse> IRefreshToken.Refresh(DataContext context, RefreshTokenRequest refreshTokenRequest)
        {
            var response = await this.refreshToken.Refresh(context, refreshTokenRequest);
            if (!response.IsSuccessful || response.Account == null || response.CurrentRefreshToken == null)
            {
                return response;
            }

            try
            {
                var newRefreshToken = this.rotateRefreshToken(context, response.CreatedByIp, refreshTokenRequest.Issuer, response.Audience);
                response.CurrentRefreshToken.ReplacedByToken = newRefreshToken.Token;
                response.Account.RefreshToken.Add(newRefreshToken);
                response.RefreshToken = new RefreshTokenDataResponse
                {
                    RefreshToken = newRefreshToken.Token,
                    Expires = newRefreshToken.Expires
                };

                response.JwtToken = this.jwtService.GenerateJwtToken(response.Account, refreshTokenRequest.Issuer, response.Audience);
            }
            catch (Exception)
            {
                response.IsSuccessful = false;
                response.RefreshTokenResponseType = RefreshTokenResponseType.duplicated;
            }

            return response;
        }

        private RefreshToken rotateRefreshToken(DataContext context, string ipAddress, string issuer, string audience)
        {
            var refreshToken = new RefreshToken
            {
                Token = getUniqueToken(),
                Expires = this.dateTimeService.UtcNow.AddDays(this.secretKeyRetrieval.GetRefreshTokenTTLInDays()),
                CreatedOn = this.dateTimeService.UtcNow,
                CreatedByIp = ipAddress,
                ReasonRevoked = string.Empty,
                ReplacedByToken = string.Empty,
                IsActive = true,
                Audience = audience,
                Issuer = issuer
            };

            return refreshToken;

            string getUniqueToken()
            {
                while (true)
                {
                    // token is a cryptographically strong random sequence of values
                    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                    // ensure token is unique by checking against db
                    var tokenIsUnique = !context.Account.Any(u => u.RefreshToken.Any(t => t.Token != null && t.Token.Trim() == token.Trim()));
                    if (!tokenIsUnique)
                    {
                        continue;
                    }

                    return token;
                }
            }
        }
    }
}