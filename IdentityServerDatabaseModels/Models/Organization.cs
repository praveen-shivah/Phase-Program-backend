namespace DatabaseContext
{
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


    public partial class Organization : BaseEntity
    {
        public Organization()
        {
            Account = new HashSet<Account>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [Column("APIKey")]
        [StringLength(100)]
        public string Apikey { get; set; } = null!;

        [InverseProperty("Organization")]
        public virtual ICollection<Account> Account { get; set; }
    }
}
