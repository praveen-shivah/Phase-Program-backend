namespace AuthenticationRepository
{
    using CommonServices;
    using DatabaseContext;

    using Microsoft.EntityFrameworkCore;

    using SecurityUtilityTypes;

    public class AuthenticateUserClearPreviousRefreshTokens : IAuthenticateUser
    {
        private readonly IAuthenticateUser authenticateUser;

        private readonly IJwtService jwtService;

        private readonly ISecretKeyRetrieval secretKeyRetrieval;

        private readonly IDateTimeService dateTimeService;

        public AuthenticateUserClearPreviousRefreshTokens(
            IAuthenticateUser authenticateUser,
            IJwtService jwtService,
            ISecretKeyRetrieval secretKeyRetrieval,
            IDateTimeService dateTimeService)
        {
            this.authenticateUser = authenticateUser;
            this.jwtService = jwtService;
            this.secretKeyRetrieval = secretKeyRetrieval;
            this.dateTimeService = dateTimeService;
        }

        async Task<AuthenticateUserResponse> IAuthenticateUser.Authenticate(DataContext context, AuthenticateUserRequest authenticateUserRequest)
        {
            var response = await this.authenticateUser.Authenticate(context, authenticateUserRequest);
            if (!response.IsSuccessful || !response.IsAuthenticated || response.Account == null)
            {
                return response;
            }

            // Clears all previous refresh tokens for this account with issuer and audience
            var query = context.RefreshToken.Where(x => x.Account == response.Account && x.Audience != null && x.Audience.Trim() == authenticateUserRequest.Audience && x.IsActive).AsQueryable();
            if (!string.IsNullOrEmpty(authenticateUserRequest.Issuer))
                query = query.Where(x => x.Issuer != null && x.Issuer.Trim() == authenticateUserRequest.Issuer);

            var list = await query.ToListAsync();
            foreach (var record in list)
            {
                record.IsActive = false;
                record.Revoked = DateTime.UtcNow;
                record.ReasonRevoked = "Authenticate";
            }

            return response;
        }
    }
}
