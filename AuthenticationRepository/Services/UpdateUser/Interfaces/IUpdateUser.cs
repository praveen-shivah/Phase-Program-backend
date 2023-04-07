namespace AuthenticationRepository
{
    using DatabaseContext;

    public interface IUpdateAccount
    {
        Task<UpdateAccountResponse> Update(DataContext context, UpdateAccountRequest updateAccountRequest);
    }
}
