namespace AuthenticationRepository
{
    using System.Security.Cryptography;

    using AuthenticationRepositoryTypes;

    using CommonServices;

    using DatabaseContext;

    using SecurityUtilityTypes;

    public class RefreshTokenRetrieveClearPreviousRefreshTokens : IRefreshToken
    {
        private readonly IDateTimeService dateTimeService;

        private readonly IJwtService jwtService;

        private readonly IRefreshToken refreshToken;

        private readonly ISecretKeyRetrieval secretKeyRetrieval;

        public RefreshTokenRetrieveClearPreviousRefreshTokens(
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
                this.revokeDescendantRefreshTokens(
                    response.CurrentRefreshToken,
                    response.Account,
                    refreshTokenRequest.IpAddress,
                    "Rotating");
            }
            catch (Exception)
            {
                response.IsSuccessful = false;
                response.RefreshTokenResponseType = RefreshTokenResponseType.duplicated;
            }

            return response;
        }

        private void revokeDescendantRefreshTokens(
            RefreshToken refreshToken,
            Account account,
            string ipAddress,
            string reason)
        {
            if (refreshToken.Token == null) return;

            var childToken = account.RefreshToken.SingleOrDefault(x => x.ReplacedByToken != null && x.ReplacedByToken.Trim() == refreshToken.Token.Trim());

            switch (childToken)
            {
                case null:
                    return;
                case { IsActive: true }:
                    this.revokeRefreshToken(
                        childToken,
                        ipAddress,
                        reason);
                    break;
                default:
                    this.revokeDescendantRefreshTokens(
                        childToken,
                        account,
                        ipAddress,
                        reason);
                    break;
            }
        }

        private void revokeRefreshToken(
            RefreshToken token,
            string ipAddress,
            string reason)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = token.ReplacedByToken;
            token.IsActive = false;
        }
    }
}