namespace AuthenticationRepository
{
    using DatabaseContext;

    using Microsoft.EntityFrameworkCore;

    public class LogoutClearPreviousRefreshTokens : ILogout
    {
        private readonly ILogout logout;

        public LogoutClearPreviousRefreshTokens(ILogout logout)
        {
            this.logout = logout;
        }

        async Task<LogoutResponse> ILogout.Logout(DataContext context, LogoutRequest logoutRequest)
        {
            var response = await this.logout.Logout(context, logoutRequest);
            if (!response.IsSuccessful || response.Account == null)
            {
                return response;
            }


            // Clears all previous refresh tokens for this account with issuer and audience
            var query = context.RefreshToken.Where(x => x.Account == response.Account && x.Audience != null && x.Audience.Trim() == logoutRequest.Audience).AsQueryable();
            if (!string.IsNullOrEmpty(logoutRequest.Issuer))
            {
                query = query.Where(x => x.Issuer != null && x.Issuer.Trim() == logoutRequest.Issuer);
            }

            var list = await query.ToListAsync();
            foreach (var record in list)
            {
                record.IsActive = false;
                record.Revoked = DateTime.UtcNow;
                record.ReasonRevoked = "Logout";
            }

            return response;
        }
    }
}
