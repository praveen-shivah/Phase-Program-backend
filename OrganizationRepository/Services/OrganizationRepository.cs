namespace OrganizationRepository
{
    using DatabaseContext;

    using DatabaseUnitOfWorkTypesLibrary;

    using Microsoft.EntityFrameworkCore;

    using OrganizationRepositoryTypes;

    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IUnitOfWorkFactory<DataContext> unitOfWorkFactory;

        private readonly IUnitOfWorkResponseFactory unitOfWorkResponseFactory;

        public OrganizationRepository(IUnitOfWorkFactory<DataContext> unitOfWorkFactory, IUnitOfWorkResponseFactory unitOfWorkResponseFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.unitOfWorkResponseFactory = unitOfWorkResponseFactory;
        }

        async Task<OrganizationResponse> IOrganizationRepository.GetOrganizationRequestAsync(OrganizationRequest organizationRequest)
        {
            var response = new OrganizationResponse();
            var uow = this.unitOfWorkFactory.Create(
                async context =>
                    {
                        var organizationRecord = await context.Organization.SingleOrDefaultAsync(x => x.Id == int.Parse(organizationRequest.OrganizationId));
                        return this.unitOfWorkResponseFactory.Create(organizationRecord != null, UOWResponseTypeEnum.cancelWithoutError);
                    });

            var result = await uow.ExecuteAsync();
            response.IsSuccessful = result.WorkItemResultEnum == WorkItemResultEnum.commitSuccessfullyCompleted;

            return response;
        }
    }
}