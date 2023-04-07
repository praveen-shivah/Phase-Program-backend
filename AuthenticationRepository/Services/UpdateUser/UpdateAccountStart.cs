namespace AuthenticationRepository
{
    using DatabaseContext;

    public class UpdateAccountStart : IUpdateAccount
    {
        Task<UpdateAccountResponse> IUpdateAccount.Update(DataContext context, UpdateAccountRequest updateAccountRequest)
        {
            return Task.FromResult(new UpdateAccountResponse { IsSuccessful = true });
        }
    }
}
