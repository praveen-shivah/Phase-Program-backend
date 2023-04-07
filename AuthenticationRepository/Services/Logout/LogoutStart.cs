namespace AuthenticationRepository
{
    using DatabaseContext;

    public class LogoutStart : ILogout
    {
        Task<LogoutResponse> ILogout.Logout(DataContext context, LogoutRequest logoutRequest)
        {
            return Task.FromResult(new LogoutResponse() { IsSuccessful = true });
        }
    }
}
