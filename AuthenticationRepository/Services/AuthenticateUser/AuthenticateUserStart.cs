namespace AuthenticationRepository
{
    using APISupportTypes;

    using DatabaseContext;

    public class AuthenticateUserStart : IAuthenticateUser
    {
        Task<AuthenticateUserResponse> IAuthenticateUser.Authenticate(DataContext context, AuthenticateUserRequest authenticateUserRequest)
        {
            return Task.FromResult(new AuthenticateUserResponse() { IsSuccessful = true, ResponseType = ResponseTypeEnum.success});
        }
    }
}
