namespace AuthenticationRepository
{
    using APISupportTypes;

    using DatabaseContext;

    using IdentityServerDatabaseModels;

    using Microsoft.EntityFrameworkCore;

    public class AuthenticateUserRetrieve : IAuthenticateUser
    {
        private readonly IAuthenticateUser authenticateUser;

        private readonly ICalculatePassword calculatePassword;

        public AuthenticateUserRetrieve(IAuthenticateUser authenticateUser, ICalculatePassword calculatePassword)
        {
            this.authenticateUser = authenticateUser;
            this.calculatePassword = calculatePassword;
        }

        async Task<AuthenticateUserResponse> IAuthenticateUser.Authenticate(DataContext context, AuthenticateUserRequest authenticateUserRequest)
        {
            var response = await this.authenticateUser.Authenticate(context, authenticateUserRequest);
            if (!response.IsSuccessful)
            {
                return response;
            }

            var account = await context.Account.Include(x => x.Organization).SingleOrDefaultAsync(x => x.OrganizationId == authenticateUserRequest.OrganizationId && x.UserName.Trim().ToLower() == authenticateUserRequest.UserName.Trim().ToLower());
            if (account == null)
            {
                response.IsAuthenticated = false;
                response.ResponseType = ResponseTypeEnum.idNotFound;
                return response;
            }

            var calculatedHash = this.calculatePassword.calculatePassword(authenticateUserRequest.Password, account.PasswordSalt.Trim());
            if (calculatedHash != account.PasswordHash.Trim())
            {
                response.IsAuthenticated = false;
                return response;
            }

            response.Account = account;
            response.IsAuthenticated = true;
            response.UserId = account.Id;
            response.UserName = account.UserName.Trim();
            response.Claims = account.Claims.Trim();

            return response;
        }
    }
}