namespace AuthenticationRepository
{
    using DatabaseContext;

    public interface IRefreshToken
    {
        Task<RefreshTokenResponse> Refresh(DataContext context, RefreshTokenRequest refreshTokenRequest);
    }
}
