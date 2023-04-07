namespace AuthenticationRepository
{
    using DatabaseContext;

    public interface IAuthenticateUser
    {
        Task<AuthenticateUserResponse> Authenticate(DataContext context, AuthenticateUserRequest authenticateUserRequest);
    }
}
