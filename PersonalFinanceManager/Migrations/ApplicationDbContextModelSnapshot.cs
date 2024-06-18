﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PersonalFinanceManager.DatabaseContext;

#nullable disable

namespace PersonalFinanceManager.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PersonalFinanceManager.Models.FinancialAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("BalanceCents")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("FinancialAccounts");
                });

            modelBuilder.Entity("PersonalFinanceManager.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TransactionId"));

                    b.Property<int>("AmountCents")
                        .HasColumnType("integer");

                    b.Property<int>("Category")
                        .HasColumnType("integer");

                    b.Property<int?>("CreditedAccountId")
                        .HasColumnType("integer");

                    b.Property<int?>("DebitedAccountId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TransactionSubcategory")
                        .HasColumnType("text");

                    b.HasKey("TransactionId");

                    b.HasIndex("CreditedAccountId");

                    b.HasIndex("DebitedAccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("PersonalFinanceManager.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<DateTime>("PasswordLastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PersonalFinanceManager.Models.FinancialAccount", b =>
                {
                    b.HasOne("PersonalFinanceManager.Models.User", "User")
                        .WithMany("FinancialAccounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalFinanceManager.Models.Transaction", b =>
                {
                    b.HasOne("PersonalFinanceManager.Models.FinancialAccount", "CreditedAccount")
                        .WithMany("IncomingTransactions")
                        .HasForeignKey("CreditedAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PersonalFinanceManager.Models.FinancialAccount", "DebitedAccount")
                        .WithMany("OutgoingTransactions")
                        .HasForeignKey("DebitedAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("CreditedAccount");

                    b.Navigation("DebitedAccount");
                });

            modelBuilder.Entity("PersonalFinanceManager.Models.FinancialAccount", b =>
                {
                    b.Navigation("IncomingTransactions");

                    b.Navigation("OutgoingTransactions");
                });

            modelBuilder.Entity("PersonalFinanceManager.Models.User", b =>
                {
                    b.Navigation("FinancialAccounts");
                });
#pragma warning restore 612, 618
        }
    }
}
