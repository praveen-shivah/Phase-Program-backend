namespace AuthenticationRepository
{
    using AuthenticationRepositoryTypes;

    using CommonServices;

    using DatabaseContext;

    public class RefreshTokenRetrieveAccount : IRefreshToken
    {
        private readonly IRefreshToken refreshToken;

        private readonly IDateTimeService dateTimeService;

        public RefreshTokenRetrieveAccount(IRefreshToken refreshToken, IDateTimeService dateTimeService)
        {
            this.refreshToken = refreshToken;
            this.dateTimeService = dateTimeService;
        }

        async Task<RefreshTokenResponse> IRefreshToken.Refresh(DataContext context, RefreshTokenRequest refreshTokenRequest)
        {
            var response = await this.refreshToken.Refresh(context, refreshTokenRequest);
            if (!response.IsSuccessful || response.CurrentRefreshToken == null)
            {
                return response;
            }

            try
            {
                response.Account = response.CurrentRefreshToken.Account;
                if (response.Account == null)
                {
                    response.IsSuccessful = false;
                    response.RefreshTokenResponseType = RefreshTokenResponseType.currentTokenNotFound;
                    return response;
                }

                response.UserName = response.Account.UserName.Trim();
                response.UserId = response.Account.Id;
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
