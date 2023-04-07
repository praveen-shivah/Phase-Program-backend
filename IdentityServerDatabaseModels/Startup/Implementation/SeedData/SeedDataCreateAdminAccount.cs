namespace IdentityServerDatabaseModels
{
    using DatabaseContext;

    using Microsoft.EntityFrameworkCore;

    public class SeedDataCreateAdminAccount : ISeedData
    {
        private readonly ISeedData seedData;

        private readonly ICalculatePassword calculatePassword;

        private readonly ICreatePasswordSalt createPasswordSalt;

        public SeedDataCreateAdminAccount(ISeedData seedData, ICalculatePassword calculatePassword, ICreatePasswordSalt createPasswordSalt)
        {
            this.seedData = seedData;
            this.calculatePassword = calculatePassword;
            this.createPasswordSalt = createPasswordSalt;
        }

        async Task<SeedDataResponse> ISeedData.SeedDataAsync(DataContext context, SeedDataRequest request)
        {
            var response = await this.seedData.SeedDataAsync(context, request);
            if (!response.IsSuccessful)
            {
                return response;
            }

            var organization = await context.Organization.SingleOrDefaultAsync(x => x.Id == 1);
            if (organization == null)
            {
                organization = new Organization()
                {
                    Id = 1,
                    Apikey = "APIKEY",
                    Name = "JPS"
                };
                context.Add(organization);
                await context.SaveChangesAsync();
            }

            var account = await context.Account.SingleOrDefaultAsync(x => x.OrganizationId == 1 && x.UserName == "admin");
            if (account == null)
            {
                var salt = this.createPasswordSalt.CreateSalt(20);
                account = new Account()
                {
                    UserName = "admin",
                    PasswordSalt = salt,
                    Claims = "Roles, 5150",
                    Organization = organization,
                    OrganizationId = organization.Id,
                    PasswordHash = this.calculatePassword.calculatePassword("admin", salt)
                };
                context.Account.Add(account);
                await context.SaveChangesAsync();
            }

            return response;
        }
    }
}
