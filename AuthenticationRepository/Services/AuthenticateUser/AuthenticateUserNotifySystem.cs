namespace AuthenticationRepository
{
    using DatabaseContext;
    using EventAggregatorLibrary;

    public class AuthenticateUserNotifySystem : IAuthenticateUser
    {
        private readonly IAuthenticateUser authenticateUser;

        private readonly ISystemEventAggregator systemEventAggregator;

        public AuthenticateUserNotifySystem(IAuthenticateUser authenticateUser, ISystemEventAggregator systemEventAggregator)
        {
            this.authenticateUser = authenticateUser;
            this.systemEventAggregator = systemEventAggregator;
        }

        async Task<AuthenticateUserResponse> IAuthenticateUser.Authenticate(DataContext context, AuthenticateUserRequest authenticateUserRequest)
        {
            var response = await this.authenticateUser.Authenticate(context, authenticateUserRequest);
            if (!response.IsSuccessful || !response.IsAuthenticated)
            {
                return response;
            }

            await this.systemEventAggregator.PublishAsync(new NotificationLogin(response.UserId));

            return response;
        }
    }

    internal class NotificationLogin
    {
        public NotificationLogin(int responseUserId)
        {
            throw new NotImplementedException();
        }
    }
}
