namespace AuthenticationRepository
{
    using ApplicationLifeCycle;

    using IdentityServerDatabaseModels;

    using SimpleInjector;

    public class CompositeRoot : CompositeRootBase
    {
        protected override bool registerBindings()
        {
            this.GlobalContainer.Register<IAuthenticationRepository, AuthenticationRepository>(Lifestyle.Transient);
            this.GlobalContainer.Register<IJwtService, JwtService>(Lifestyle.Transient);

            this.GlobalContainer.Register<IAuthenticateUser, AuthenticateUserStart>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IAuthenticateUser, AuthenticateUserRetrieve>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IAuthenticateUser, AuthenticateUserClearPreviousRefreshTokens>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IAuthenticateUser, AuthenticateUserGenerateJwt>(Lifestyle.Transient);

            this.GlobalContainer.Register<IRefreshToken, RefreshTokenStart>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IRefreshToken, RefreshTokenRetrieveRefreshToken>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IRefreshToken, RefreshTokenRetrieveAccount>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IRefreshToken, RefreshTokenRetrieveClearPreviousRefreshTokens>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IRefreshToken, RefreshTokenRetrieveRotateRefreshToken>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IRefreshToken, RefreshTokenValidateExpiration>(Lifestyle.Transient);

            this.GlobalContainer.Register<ILogout, LogoutStart>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<ILogout, LogoutRetrieveUser>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<ILogout, LogoutClearPreviousRefreshTokens>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<ILogout, LogoutNotifySystem>(Lifestyle.Transient);

            this.GlobalContainer.Register<IUpdateAccount, UpdateAccountStart>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IUpdateAccount, UpdateAccountValidateClaims>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IUpdateAccount, UpdateAccountRetrieveAccount>(Lifestyle.Transient);
            this.GlobalContainer.RegisterDecorator<IUpdateAccount, UpdateAccountUpdate>(Lifestyle.Transient);

            return true;
        }
    }
}
