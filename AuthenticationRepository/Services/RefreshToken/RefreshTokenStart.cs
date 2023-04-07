namespace AuthenticationRepository
{
    using AuthenticationRepositoryTypes;

    using DatabaseContext;

    public class RefreshTokenStart : IRefreshToken
    {
        Task<RefreshTokenResponse> IRefreshToken.Refresh(DataContext context, RefreshTokenRequest refreshTokenRequest)
        {
            return Task.FromResult(new RefreshTokenResponse()
            {
                IsSuccessful = true,
                RefreshTokenResponseType = RefreshTokenResponseType.successful
            });
        }
    }
}
