namespace AuthenticationRepository
{
    using DatabaseContext;

    public interface ILogout
    {
        Task<LogoutResponse> Logout(DataContext context, LogoutRequest logoutRequest);
    }
}
