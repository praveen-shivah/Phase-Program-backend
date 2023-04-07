namespace AuthenticationRepository
{
    using DatabaseContext;

    using Microsoft.EntityFrameworkCore;

    public class UpdateAccountRetrieveAccount : IUpdateAccount
    {
        private readonly IUpdateAccount updateUser;

        public UpdateAccountRetrieveAccount(IUpdateAccount updateUser)
        {
            this.updateUser = updateUser;
        }

        async Task<UpdateAccountResponse> IUpdateAccount.Update(DataContext context, UpdateAccountRequest updateAccountRequest)
        {
            var response = await this.updateUser.Update(context, updateAccountRequest);
            if (!response.IsSuccessful)
            {
                return response;
            }

            var account = await context.Account.Include(o => o.Organization).SingleOrDefaultAsync(x => x.OrganizationId == updateAccountRequest.OrganizationId && x.UserName.Trim() == updateAccountRequest.AccountDto.UserName.Trim());
            if (account == null)
            {
                if (updateAccountRequest.AccountDto.UserName.ToUpper() == AuthenticationConstants.AuthenticationOperatorDefaultUserName.ToUpper())
                {
                    response.IsSuccessful = false;
                    return response;
                }

                var organization = await context.Organization.SingleAsync(o => o.Id == updateAccountRequest.OrganizationId);
                account = new Account
                {
                    Organization = organization,
                    UserName = updateAccountRequest.AccountDto.UserName
                };
                context.Account.Add(account);
            }

            response.Account = account;

            return response;
        }
    }
}