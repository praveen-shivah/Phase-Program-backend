using DatabaseContext;
using IdentityServerDatabaseModels.Startup.Implementation.CreateAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationRepository.Services.CreateUser.Interfaces
{
    public interface ICreateUser
    {
        Task<CreateAccountResoponse> CreateUser(DataContext context, CreateAccountRequest request, string claims);
    }
}
