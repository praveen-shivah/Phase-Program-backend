namespace OrganizationRepository
{
    using ApplicationLifeCycle;

    using OrganizationRepositoryTypes;

    using SimpleInjector;

    public class CompositeRoot : CompositeRootBase
    {
        protected override bool registerBindings()
        {
            this.GlobalContainer.Register<IOrganizationRepository, OrganizationRepository>(Lifestyle.Transient);

            return true;
        }
    }
}
