namespace IdentityServerDatabaseModels
{
    using ApplicationLifeCycle;

    using DatabaseContext;
    using DatabaseUnitOfWorkClassLibrary;

    using DatabaseUnitOfWorkLibrary;

    using DatabaseUnitOfWorkTypesLibrary;

    using SimpleInjector;

    public class CompositionRoot : CompositeRootBase
    {
        protected override bool registerBindings()
        {
            this.GlobalContainer.Register<IUnitOfWorkFactory<DataContext>, UnitOfWorkFactory<DataContext>>(Lifestyle.Singleton);
            this.GlobalContainer.Register<IUnitOfWorkResponseFactory, UnitOfWorkResponseFactory>(Lifestyle.Singleton);
            this.GlobalContainer.Register<IUnitOfWorkContextContainerFactory<DataContext>, UnitOfWorkContextContainerFactory<DataContext>>(Lifestyle.Singleton);
            this.GlobalContainer.Register<IWorkItemFactory<DataContext>, WorkItemFactory<DataContext>>(Lifestyle.Singleton);
            this.GlobalContainer.Collection.Append<IRequestLifeCycleStartupItem, RequestLifeCycleStartupItemSeedData>(Lifestyle.Singleton);

            this.GlobalContainer.Register<ISeedData, SeedDataStart>(Lifestyle.Singleton);
            this.GlobalContainer.RegisterDecorator<ISeedData, SeedDataCreateAdminAccount>(Lifestyle.Singleton);

            return true;
        }
    }
}
