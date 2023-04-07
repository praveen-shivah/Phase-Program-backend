namespace DatabaseContext
{
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


[Index(nameof(Issuer), nameof(Audience), nameof(ReplacedByToken), Name = "IX_RefreshToken_Issuer_Audience_ReplacedByToken")]
[Index(nameof(Accountid), Name = "fki_FK_RefreshToken_Account")]
    public partial class RefreshToken : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        [StringLength(200)]
        public string CreatedByIp { get; set; } = null!;
        public DateTime Expires { get; set; }
        [Column("revoked")]
        public DateTime? Revoked { get; set; }
        [StringLength(20)]
        public string? RevokedByIp { get; set; }
        [Column("accountid")]
        public int Accountid { get; set; }
        [StringLength(200)]
        public string? Token { get; set; }
        [StringLength(100)]
        public string? ReasonRevoked { get; set; }
        [StringLength(200)]
        public string? ReplacedByToken { get; set; }
        public bool IsActive { get; set; }
        [StringLength(200)]
        public string? Issuer { get; set; }
        [StringLength(200)]
        public string? Audience { get; set; }

        [ForeignKey(nameof(Accountid))]
        [InverseProperty("RefreshToken")]
        public virtual Account Account { get; set; } = null!;
    }
}
