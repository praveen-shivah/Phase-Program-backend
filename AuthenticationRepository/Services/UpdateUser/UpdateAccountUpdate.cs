namespace AuthenticationRepository
{
    using DatabaseContext;

    using IdentityServerDatabaseModels;

    public class UpdateAccountUpdate : IUpdateAccount
    {
        private readonly ICalculatePassword calculatePassword;

        private readonly ICreatePasswordSalt createPasswordSalt;

        private readonly IUpdateAccount updateUser;

        public UpdateAccountUpdate(
            IUpdateAccount updateUser,
            ICalculatePassword calculatePassword,
            ICreatePasswordSalt createPasswordSalt)
        {
            this.updateUser = updateUser;
            this.calculatePassword = calculatePassword;
            this.createPasswordSalt = createPasswordSalt;
        }

        async Task<UpdateAccountResponse> IUpdateAccount.Update(
            DataContext context,
            UpdateAccountRequest updateAccountRequest)
        {
            var response = await this.updateUser.Update(context, updateAccountRequest);
            if (!response.IsSuccessful || response.Account == null)
            {
                return response;
            }

            if (response.Account.UserName.Trim() == AuthenticationConstants.AuthenticationOperatorDefaultUserName.ToUpper())
            {
                return response;
            }

            response.Account.UserName = updateAccountRequest.AccountDto.UserName;
            response.Account.Claims = updateAccountRequest.AccountDto.Claims;
            var salt = this.createPasswordSalt.CreateSalt(20);;
            response.Account.PasswordSalt = salt;
            response.Account.PasswordHash = this.calculatePassword.calculatePassword(updateAccountRequest.AccountDto.Password, response.Account.PasswordSalt);

            return response;
        }
    }
}