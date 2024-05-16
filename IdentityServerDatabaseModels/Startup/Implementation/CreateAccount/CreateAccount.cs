using DatabaseContext;
using IdentityServerDatabaseModels.Startup.Implementation.CreateAccount.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerDatabaseModels.Startup.Implementation.CreateAccount
{
    public class CreateAccount : ICreateAccount
    {
        private readonly ICalculatePassword _calculatePassword;
        private readonly ICreatePasswordSalt _createPasswordSalt;
        private readonly DataContext context;
        public CreateAccount(ICalculatePassword calculatePassword,ICreatePasswordSalt createPasswordSalt,
            DataContext context)
        {
            _calculatePassword = calculatePassword;   
            _createPasswordSalt = createPasswordSalt;
            this.context = context;
        }
        public async Task<CreateAccountResoponse> RegisterAccountAsync(CreateAccountRequest request)
        {
            var organisation =  await context.Organization.SingleOrDefaultAsync(x => x.Id == request.OrganisationId);
            if(organisation == null)
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
            if(account == null) 
            {
                var salt = _createPasswordSalt.CreateSalt(25);
                account = new Account()
                {
                    UserName = request.UserName,
                    PasswordSalt = salt,
                    PasswordHash = _calculatePassword.calculatePassword(request.Password,salt),
                    Claims = request.Claims,
                    Organization = organisation,
                    OrganizationId = request.OrganisationId
                };
                context.Account.Add(account);
                await context.SaveChangesAsync();
            }
            return new CreateAccountResoponse()
            {
                IsSuccessful = true
            };
        }

        //public Task<CreateAccountResoponse> RegisterAccountAsync(DataContext context, CreateAccountResoponse resoponse)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
