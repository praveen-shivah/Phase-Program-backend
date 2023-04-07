namespace DatabaseContext
{
    using System;
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    public partial class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; } = null!;
        public virtual DbSet<Organization> Organization { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshToken { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasIdentityOptions(1000000L, null, null, null, null, null);

                entity.Property(e => e.Claims).HasDefaultValueSql("''::character varying");

                entity.Property(e => e.PasswordHash).IsFixedLength();

                entity.Property(e => e.PasswordSalt).IsFixedLength();

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Organization");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Apikey).IsFixedLength();

                entity.Property(e => e.Name).IsFixedLength();
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasIdentityOptions(1000000L, null, null, null, null, null);

                entity.Property(e => e.Audience).IsFixedLength();

                entity.Property(e => e.Issuer).IsFixedLength();

                entity.Property(e => e.ReasonRevoked).IsFixedLength();

                entity.Property(e => e.ReplacedByToken).IsFixedLength();

                entity.Property(e => e.RevokedByIp).IsFixedLength();

                entity.Property(e => e.Token).IsFixedLength();

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.RefreshToken)
                    .HasForeignKey(d => d.Accountid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RefreshToken_Account");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
