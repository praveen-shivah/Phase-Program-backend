namespace IdentityServerDatabaseModels
{
    using ApplicationLifeCycle;

    using DatabaseContext;

    using DatabaseUnitOfWorkTypesLibrary;

    public class RequestLifeCycleStartupItemSeedData : IRequestLifeCycleStartupItem
    {
        private readonly ISeedData seedData;

        private readonly IUnitOfWorkFactory<DataContext> unitOfWorkFactory;

        private readonly IUnitOfWorkResponseFactory unitOfWorkResponseFactory;

        public RequestLifeCycleStartupItemSeedData(
            IUnitOfWorkFactory<DataContext> unitOfWorkFactory,
            ISeedData seedData,
            IUnitOfWorkResponseFactory unitOfWorkResponseFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.seedData = seedData;
            this.unitOfWorkResponseFactory = unitOfWorkResponseFactory;
        }

        RequestLifeCycleStartupItemPriority IRequestLifeCycleStartupItem.RequestLifeCycleStartupItemPriority => RequestLifeCycleStartupItemPriority.seedingData;

        async Task<bool> IRequestLifeCycleStartupItem.ExecuteAsync()
        {
            var result = false;
            var uow = this.unitOfWorkFactory.Create(
                async context =>
                    {
                        var response = await this.seedData.SeedDataAsync(context, new SeedDataRequest());
                        return this.unitOfWorkResponseFactory.Create(response.IsSuccessful, UOWResponseTypeEnum.doneOrRollback);
                    });
            var executeResult = await uow.ExecuteAsync();

            result = executeResult.WorkItemResultEnum == WorkItemResultEnum.commitSuccessfullyCompleted;

            return result;
        }
    }
}