namespace AuthenticationRepository
{
    using AuthenticationDto;

    public class UpdateAccountRequest
    {
        public UpdateAccountRequest(int organizationId, AccountUpdateDto accountDto)
        {
            this.OrganizationId = organizationId;
            this.AccountDto = accountDto;
        }

        public int OrganizationId { get; }

        public AccountUpdateDto AccountDto { get; }
    }
}