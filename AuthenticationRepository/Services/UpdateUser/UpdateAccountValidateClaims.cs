namespace AuthenticationRepository
{
    using DatabaseContext;

    using IdentityServerDatabaseModels;

    public class UpdateAccountValidateClaims : IUpdateAccount
    {
        private readonly ICalculatePassword calculatePassword;

        private readonly ICreatePasswordSalt createPasswordSalt;

        private readonly IUpdateAccount updateUser;

        public UpdateAccountValidateClaims(
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
            if (!response.IsSuccessful)
            {
                return response;
            }

            // Claims must be in the form of "key","pair"
            // In other words, in multiples of two 
            var claimsInfo = updateAccountRequest.AccountDto.Claims.Split(',');
            if(claimsInfo.Length < 2 || claimsInfo.Length % 2 != 0)
            {
                response.IsSuccessful = false;
            }

            return response;
        }
    }
}