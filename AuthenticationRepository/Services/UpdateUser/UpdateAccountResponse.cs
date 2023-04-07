namespace AuthenticationRepository
{
    using DatabaseContext;

    public class UpdateAccountResponse
    {
        public bool IsSuccessful { get; set; }

        public Account? Account { get; set; }
    }
}
