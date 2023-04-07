namespace IdentityServerDatabaseSupport
{
    using ApplicationLifeCycle;

    using DatabaseContext;
    using DatabaseUnitOfWorkTypesLibrary;

    using SharedUtilities;

    using SimpleInjector;

    public class CompositionRoot : CompositeRootBase
    {
        protected override bool registerBindings()
        {
            this.GlobalContainer.Register<IEntityContextFrameWorkFactory<DataContext>, EntityContextFrameWorkFactoryNormal>(Lifestyle.Singleton);
            this.GlobalContainer.Register<IConnectionFactory, ConnectionFactoryNormal>(Lifestyle.Singleton);

            return true;
        }
    }
}
