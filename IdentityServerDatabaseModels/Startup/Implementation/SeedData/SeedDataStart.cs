namespace IdentityServerDatabaseModels
{
    using DatabaseContext;

    public class SeedDataStart : ISeedData
    {
        Task<SeedDataResponse> ISeedData.SeedDataAsync(DataContext context, SeedDataRequest request)
        {
            return Task.FromResult(new SeedDataResponse() { IsSuccessful = true });
        }
    }
}
