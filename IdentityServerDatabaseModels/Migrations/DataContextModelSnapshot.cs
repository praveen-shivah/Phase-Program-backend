﻿// <auto-generated />
using System;
using DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IdentityServerDatabaseModels.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DatabaseContext.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));
                    NpgsqlPropertyBuilderExtensions.HasIdentityOptions(b.Property<int>("Id"), 1000000L, null, null, null, null, null);

                    b.Property<string>("Claims")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasDefaultValueSql("''::character varying");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character(100)")
                        .IsFixedLength();

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "OrganizationId" }, "fki_FK_Account_Organization");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("DatabaseContext.Organization", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Apikey")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character(100)")
                        .HasColumnName("APIKey")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character(100)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("DatabaseContext.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));
                    NpgsqlPropertyBuilderExtensions.HasIdentityOptions(b.Property<int>("Id"), 1000000L, null, null, null, null, null);

                    b.Property<int>("Accountid")
                        .HasColumnType("integer")
                        .HasColumnName("accountid");

                    b.Property<string>("Audience")
                        .HasMaxLength(200)
                        .HasColumnType("character(200)")
                        .IsFixedLength();

                    b.Property<string>("CreatedByIp")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Issuer")
                        .HasMaxLength(200)
                        .HasColumnType("character(200)")
                        .IsFixedLength();

                    b.Property<string>("ReasonRevoked")
                        .HasMaxLength(100)
                        .HasColumnType("character(100)")
                        .IsFixedLength();

                    b.Property<string>("ReplacedByToken")
                        .HasMaxLength(200)
                        .HasColumnType("character(200)")
                        .IsFixedLength();

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("revoked");

                    b.Property<string>("RevokedByIp")
                        .HasMaxLength(20)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("Token")
                        .HasMaxLength(200)
                        .HasColumnType("character(200)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Issuer", "Audience", "ReplacedByToken" }, "IX_RefreshToken_Issuer_Audience_ReplacedByToken");

                    b.HasIndex(new[] { "Accountid" }, "fki_FK_RefreshToken_Account");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("DatabaseContext.Account", b =>
                {
                    b.HasOne("DatabaseContext.Organization", "Organization")
                        .WithMany("Account")
                        .HasForeignKey("OrganizationId")
                        .IsRequired()
                        .HasConstraintName("FK_Account_Organization");

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("DatabaseContext.RefreshToken", b =>
                {
                    b.HasOne("DatabaseContext.Account", "Account")
                        .WithMany("RefreshToken")
                        .HasForeignKey("Accountid")
                        .IsRequired()
                        .HasConstraintName("FK_RefreshToken_Account");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("DatabaseContext.Account", b =>
                {
                    b.Navigation("RefreshToken");
                });

            modelBuilder.Entity("DatabaseContext.Organization", b =>
                {
                    b.Navigation("Account");
                });
#pragma warning restore 612, 618
        }
    }
}
