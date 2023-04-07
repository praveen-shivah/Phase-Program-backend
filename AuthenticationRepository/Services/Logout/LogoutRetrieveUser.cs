namespace AuthenticationRepository
{
    using DatabaseContext;

    using Microsoft.EntityFrameworkCore;

    public class LogoutRetrieveUser : ILogout
    {
        private readonly ILogout logout;

        public LogoutRetrieveUser(ILogout logout)
        {
            this.logout = logout;
        }

        async Task<LogoutResponse> ILogout.Logout(DataContext context, LogoutRequest logoutRequest)
        {
            var response = await this.logout.Logout(context, logoutRequest);
            if (!response.IsSuccessful)
            {
                return response;
            }

            var account = context.Account.Include(r => r.RefreshToken).Include(o => o.Organization)
                .SingleOrDefault(x => x.OrganizationId == logoutRequest.OrganizationId && x.UserName.Trim() == logoutRequest.UserName.Trim());

            if (response.Account == null)
            {
                response.IsSuccessful = false;
                return response;
            }

            return response;
        }
    }
}
