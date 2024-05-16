using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerDatabaseModels.Startup.Implementation.CreateAccount
{
    public class CreateAccountRequest
    {
        public string  UserName { get; set; }
        public string Claims { get; set; }
        public string Password { get; set; }
        public int OrganisationId { get; set; }
    }
}
