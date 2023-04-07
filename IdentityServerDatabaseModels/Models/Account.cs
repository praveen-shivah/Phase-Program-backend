namespace DatabaseContext
{
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


[Index(nameof(OrganizationId), Name = "fki_FK_Account_Organization")]
    public partial class Account : BaseEntity
    {
        public Account()
        {
            RefreshToken = new HashSet<RefreshToken>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(200)]
        public string UserName { get; set; } = null!;
        public int OrganizationId { get; set; }
        [StringLength(100)]
        public string PasswordHash { get; set; } = null!;
        [StringLength(20)]
        public string PasswordSalt { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
        [StringLength(200)]
        public string Claims { get; set; } = null!;

        [ForeignKey(nameof(OrganizationId))]
        [InverseProperty("Account")]
        public virtual Organization Organization { get; set; } = null!;
        [InverseProperty("Account")]
        public virtual ICollection<RefreshToken> RefreshToken { get; set; }
    }
}
