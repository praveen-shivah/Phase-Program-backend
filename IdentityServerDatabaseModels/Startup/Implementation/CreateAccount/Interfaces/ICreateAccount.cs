using DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerDatabaseModels.Startup.Implementation.CreateAccount.Interfaces
{
    public interface ICreateAccount
    {
        Task<CreateAccountResoponse> RegisterAccountAsync( CreateAccountRequest request);
    }
}
