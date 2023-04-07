namespace AuthenticationRepository
{
    using DatabaseContext;

    using EventAggregatorLibrary;

    public class LogoutNotifySystem : ILogout
    {
        private readonly ILogout logout;

        private readonly ISystemEventAggregator systemEventAggregator;

        public LogoutNotifySystem(ILogout logout, ISystemEventAggregator systemEventAggregator)
        {
            this.logout = logout;
            this.systemEventAggregator = systemEventAggregator;
        }

        async Task<LogoutResponse> ILogout.Logout(DataContext context, LogoutRequest logoutRequest)
        {
            var response = await this.logout.Logout(context, logoutRequest);
            if (!response.IsSuccessful)
            {
                return response;
            }

            await this.systemEventAggregator.PublishAsync(new NotificationLogout());

            return response;
        }
    }

    internal class NotificationLogout
    {
    }
}
