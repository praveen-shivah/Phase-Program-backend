using AuthenticationRepository.Services.CreateUser.Interfaces;
using DatabaseContext;
using IdentityServerDatabaseModels;
using IdentityServerDatabaseModels.Startup.Implementation.CreateAccount;
using IdentityServerDatabaseModels.Startup.Implementation.CreateAccount.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationRepository.Services.CreateUser
{
    public class CreateUserInsertion : ICreateUser
    {
        private readonly ICreatePasswordSalt createPasswordSalt;
        private readonly ICalculatePassword calculatePassword;
        public CreateUserInsertion(
            ICreatePasswordSalt createPasswordSalt,
            ICalculatePassword calculatePassword)
        {
            this.createPasswordSalt = createPasswordSalt;
            this.calculatePassword = calculatePassword;
        }
        async Task<CreateAccountResoponse> ICreateUser.CreateUser(DataContext context, CreateAccountRequest request, string claims)
        {

            try
            {
            var organisation = await context.Organization.SingleOrDefaultAsync(x => x.Id == request.OrganisationId);
            if (organisation == null)
            {
                organisation = new Organization()
                {
                    Id = 1,
                    Apikey = "APIKEY",
                    Name = "JPS"
                };
                context.Add(organisation);
                await context.SaveChangesAsync();
            }

            var account = await context.Account.SingleOrDefaultAsync(x => x.OrganizationId == request.OrganisationId && x.UserName == request.UserName);
            if (account == null)
            {
                var salt = createPasswordSalt.CreateSalt(20);
                account = new Account()
                {
                    UserName = request.UserName,
                    PasswordSalt = salt,
                    PasswordHash = calculatePassword.calculatePassword(request.Password, salt),
                    Claims = claims,
                    Organization = organisation,
                    OrganizationId = request.OrganisationId
                };
                context.Account.Add(account);
                 var reponse = await context.SaveChangesAsync();
            }
            return new CreateAccountResoponse
            {
                IsSuccessful = true
            };

            }

            catch (Exception ex)
            {
                throw;
            }

        }

       
    }
}
