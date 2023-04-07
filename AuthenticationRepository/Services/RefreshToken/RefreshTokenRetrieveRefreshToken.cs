namespace AuthenticationRepository
{
    using AuthenticationRepositoryTypes;

    using CommonServices;

    using DatabaseContext;

    using Microsoft.EntityFrameworkCore;

    public class RefreshTokenRetrieveRefreshToken : IRefreshToken
    {
        private readonly IRefreshToken refreshToken;

        private readonly IDateTimeService dateTimeService;

        public RefreshTokenRetrieveRefreshToken(IRefreshToken refreshToken, IDateTimeService dateTimeService)
        {
            this.refreshToken = refreshToken;
            this.dateTimeService = dateTimeService;
        }

        async Task<RefreshTokenResponse> IRefreshToken.Refresh(DataContext context, RefreshTokenRequest refreshTokenRequest)
        {
            var response = await this.refreshToken.Refresh(context, refreshTokenRequest);
            if (!response.IsSuccessful)
            {
                return response;
            }

            try
            {
                response.CurrentRefreshToken = await context.RefreshToken.Include(x => x.Account).SingleOrDefaultAsync(x => x.Token != null && x.Token.Trim() == refreshTokenRequest.RefreshToken.Trim());
                if (response.CurrentRefreshToken == null)
                {
                    response.IsSuccessful = false;
                    response.RefreshTokenResponseType = RefreshTokenResponseType.currentTokenNotFound;
                    return response;
                }

                response.Audience = response.CurrentRefreshToken.Audience ?? string.Empty;
                response.CreatedByIp = response.CurrentRefreshToken.CreatedByIp;
            }
            catch
            {
                response.IsSuccessful = false;
                response.RefreshTokenResponseType = RefreshTokenResponseType.duplicated;
            }

            return response;
        }
    }
}
