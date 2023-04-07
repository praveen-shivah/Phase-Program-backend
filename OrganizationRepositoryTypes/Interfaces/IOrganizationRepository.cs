namespace OrganizationRepositoryTypes
{
    public interface IOrganizationRepository
    {
        Task<OrganizationResponse> GetOrganizationRequestAsync(OrganizationRequest organizationRequest);
    }
}