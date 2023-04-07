namespace IdentityServerDatabaseModels

{
    using DatabaseContext;

    public interface ISeedData
    {
        Task<SeedDataResponse> SeedDataAsync(DataContext context, SeedDataRequest request);
    }
}
