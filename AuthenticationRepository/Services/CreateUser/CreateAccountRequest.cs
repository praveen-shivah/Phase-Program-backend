using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationRepository.Services.CreateUser
{
    public class CreateAccountRequest
    {
        [Required]
        [StringLength(13, ErrorMessage = "Description Max Length is 13")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$")]
        public string UserName { get; set; }
        public string? NickName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int OrganisationId { get; set; }
    }
}
