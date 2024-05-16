namespace AuthenticationRepository
{
   using APISupportTypes;

    using AuthenticationDto;
    using AuthenticationRepositoryTypes;

    using DatabaseContext;

    using DatabaseUnitOfWorkTypesLibrary;
    using Services.CreateUser.Interfaces;
    using Services.CreateUser;
    using IdentityServerDatabaseModels;
   using Microsoft.EntityFrameworkCore;

    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IAuthenticateUser authenticateUser;
        private readonly ICreateUser createUser;

        private readonly ILogout logout;

        private readonly IRefreshToken refreshToken;

        private readonly IUnitOfWorkFactory<DataContext> unitOfWorkFactory;

        private readonly IUnitOfWorkResponseFactory unitOfWorkResponseFactory;

        private readonly IUpdateAccount updateUser;


        public AuthenticationRepository(
            IUnitOfWorkFactory<DataContext> unitOfWorkFactory,
            IUnitOfWorkResponseFactory unitOfWorkResponseFactory,
            IAuthenticateUser authenticateUser,
            IRefreshToken refreshToken,
            ILogout logout,
            IUpdateAccount updateUser,ICreateUser createUser)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.unitOfWorkResponseFactory = unitOfWorkResponseFactory;
            this.authenticateUser = authenticateUser;
            this.refreshToken = refreshToken;
            this.logout = logout;
            this.updateUser = updateUser;
            this.createUser = createUser;
        }

        

        async Task<AuthenticationResponse> IAuthenticationRepository.Authenticate(AuthenticationRequest authenticationRequest)
        {
            AuthenticateUserResponse? authenticateUserResponse = null;
            var uow = this.unitOfWorkFactory.Create(
                async context =>
                    {
                        authenticateUserResponse = await this.authenticateUser.Authenticate(
                                                       context,
                                                       new AuthenticateUserRequest
                                                       {
                                                           OrganizationId = authenticationRequest.OrganizationId,
                                                           UserName = authenticationRequest.UserId,
                                                           Password = authenticationRequest.Password,
                                                           IpAddress = authenticationRequest.IpAddress,
                                                           Audience = authenticationRequest.Audience,
                                                           Issuer = authenticationRequest.Issuer
                                                       });

                        return this.unitOfWorkResponseFactory.Create(true, UOWResponseTypeEnum.doneOrRollback);
                    });

            var result = await uow.ExecuteAsync();
            if (result.WorkItemResultEnum != WorkItemResultEnum.commitSuccessfullyCompleted || authenticateUserResponse == null)
            {
                return new AuthenticationResponse
                {
                    IsSuccessful = false,
                    IsAuthenticated = false,
                    Claims = string.Empty,
                    UserName = string.Empty,
                    JwtToken = string.Empty,
                    ResponseType = ResponseTypeEnum.databaseError,
                    ErrorMessage = result.ErrorMessage
                };
            }

            if (authenticateUserResponse.IsSuccessful && authenticateUserResponse.IsAuthenticated && authenticateUserResponse.RefreshToken != null)
            {
                return new AuthenticationResponse
                {
                    UserName = authenticateUserResponse.UserName,
                    IsAuthenticated = authenticateUserResponse.IsAuthenticated,
                    IsSuccessful = true,
                    Claims = authenticateUserResponse.Claims,
                    JwtToken = authenticateUserResponse.JwtToken,
                    RefreshToken = new RefreshTokenDataResponse()
                    {
                        Expires = authenticateUserResponse.RefreshToken.Expires,
                        RefreshToken = authenticateUserResponse.RefreshToken?.Token ?? string.Empty,
                    },
                    ResponseType = authenticateUserResponse.ResponseType,
                    ErrorMessage = authenticateUserResponse.ErrorMessage
                };
            }

            return new AuthenticationResponse
            {
                UserName = authenticateUserResponse.UserName,
                IsAuthenticated = authenticateUserResponse.IsAuthenticated,
                IsSuccessful = true,
                Claims = string.Empty,
                ResponseType = authenticateUserResponse.ResponseType,
                ErrorMessage = authenticateUserResponse.ErrorMessage
            };
        }

        async Task<AuthenticationResponse> IAuthenticationRepository.GetUserByUserName(int organizationId, string userName)
        {
            AuthenticationResponse? authenticationResponse = null;
            var uow = this.unitOfWorkFactory.Create(
                async context =>
                    {
                        var account = await context.Account.Include(o => o.Organization).SingleOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserName.ToUpper().Trim() == userName.ToUpper().Trim());
                        if (account == null)
                        {
                            authenticationResponse = new AuthenticationResponse
                            {
                                IsAuthenticated = false,
                                IsSuccessful = false
                            };
                        }
                        else
                        {
                            authenticationResponse = new AuthenticationResponse
                            {
                                IsAuthenticated = true,
                                IsSuccessful = true,
                                OrganizationId = account.OrganizationId,
                                UserName = account.UserName,
                                Claims = account.Claims
                            };
                        }

                        return this.unitOfWorkResponseFactory.Create(true, UOWResponseTypeEnum.doneOrRollback);
                    });

            var result = await uow.ExecuteAsync();
            if (result.WorkItemResultEnum != WorkItemResultEnum.commitSuccessfullyCompleted || authenticationResponse == null)
            {
                return new AuthenticationResponse
                {
                    IsSuccessful = false,
                    IsAuthenticated = false
                };
            }

            return authenticationResponse;
        }

         async Task<CreateAccountResoponse> IAuthenticationRepository.CreateAccount(CreateAccountRequest request, string claims)
        {
            CreateAccountResoponse? createaccountResponse = null;
            var uow = this.unitOfWorkFactory.Create(
                async context =>
                {
                    createaccountResponse = await this.createUser.CreateUser(
                                                   context, request, claims
                                                   );

                    return this.unitOfWorkResponseFactory.Create(true, UOWResponseTypeEnum.doneOrRollback);
                });
            var result = await uow.ExecuteAsync();
            if (result.WorkItemResultEnum != WorkItemResultEnum.commitSuccessfullyCompleted || createaccountResponse == null)
            {
                return new CreateAccountResoponse()
                {
                    IsSuccessful = true
                };
            }

            return createaccountResponse;

        }
    

        async Task<LogoutResponse> IAuthenticationRepository.Logout(LogoutRequest logoutRequest)
        {
            var logoutResponse = new LogoutResponse { IsSuccessful = true };
            var uow = this.unitOfWorkFactory.Create(
                async context =>
                    {
                        logoutResponse = await this.logout.Logout(context, logoutRequest);
                        return this.unitOfWorkResponseFactory.Create(true, UOWResponseTypeEnum.doneOrRollback);
                    });

            var result = await uow.ExecuteAsync();
            if (result.WorkItemResultEnum != WorkItemResultEnum.commitSuccessfullyCompleted)
            {
                return new LogoutResponse { IsSuccessful = false };
            }

            return logoutResponse;
        }

        async Task<RefreshTokenResponse> IAuthenticationRepository.RefreshToken(
            string refreshToken,
            string ipAddress,
            string issuer)
        {
            RefreshTokenResponse? refreshTokenResponse = null;
            var uow = this.unitOfWorkFactory.Create(
                async context =>
                    {
                        refreshTokenResponse = await this.refreshToken.Refresh(
                                                   context,
                                                   new RefreshTokenRequest
                                                   {
                                                       RefreshToken = refreshToken,
                                                       IpAddress = ipAddress,
                                                       Issuer = issuer
                                                   });
                        return this.unitOfWorkResponseFactory.Create(true, UOWResponseTypeEnum.doneOrRollback);
                    });

            var result = await uow.ExecuteAsync();
            if (result.WorkItemResultEnum != WorkItemResultEnum.commitSuccessfullyCompleted || refreshTokenResponse == null)
            {
                return new RefreshTokenResponse { IsSuccessful = false };
            }

            return refreshTokenResponse;
        }

        async Task<UpdateAccountResponse> IAuthenticationRepository.UpdateUser(int organizationId, AccountUpdateDto accountDto)
        {
            var uow = this.unitOfWorkFactory.Create(
                async context =>
                    {
                        var result = await this.updateUser.Update(context, new UpdateAccountRequest(organizationId, accountDto));
                        return this.unitOfWorkResponseFactory.Create(result.IsSuccessful, UOWResponseTypeEnum.doneOrRollback);
                    });
            var result = await uow.ExecuteAsync();

            if (result.WorkItemResultEnum != WorkItemResultEnum.commitSuccessfullyCompleted)
            {
                return new UpdateAccountResponse { IsSuccessful = false };
            }

            return new UpdateAccountResponse { IsSuccessful = true };
        }
    }
}