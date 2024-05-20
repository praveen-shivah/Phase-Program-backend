using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationRepository.Services.CreateUser
{
    public class CreateAccountRequest
    {
        public string UserName { get; set; }
        public string? NickName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int OrganisationId { get; set; }
    }
}
