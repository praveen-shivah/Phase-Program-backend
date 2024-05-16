namespace AuthenticationRepository
{
    using AuthenticationDto;

    using AuthenticationRepositoryTypes;
    using global::AuthenticationRepository.Services.CreateUser;

    public interface IAuthenticationRepository
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest authenticationRequest);

        Task<AuthenticationResponse> GetUserByUserName(int organizationId, string userName);

        Task<RefreshTokenResponse> RefreshToken(string refreshToken, string ipAddress, string issuer);

        Task<LogoutResponse> Logout(LogoutRequest logoutRequest);

        Task<UpdateAccountResponse> UpdateUser(int organizationId, AccountUpdateDto accountDto);
        //Task<CreateAccountResoponse> RegisterAccountAsync(CreateAccountRequest request);
        Task<CreateAccountResoponse> CreateAccount(CreateAccountRequest request, string claims);
    }
}
