namespace AuthenticationRepository
{
    using DatabaseContext;

    public class LogoutResponse
    {
        public bool IsSuccessful { get; set; }

        public Account? Account { get; set; }
    }
}
