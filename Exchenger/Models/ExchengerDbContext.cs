using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Exchenger.Models;

public partial class ExchengerDbContext : DbContext
{
    public ExchengerDbContext()
    {
    }

    public ExchengerDbContext(DbContextOptions<ExchengerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<CurrencyAmount> CurrencyAmounts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerRole> CustomerRoles { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=CurrencyExchanger;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurId).HasName("PK__Currency__A2011227DFB29AAE");

            entity.ToTable("Currency");

            entity.Property(e => e.CurId)
                .ValueGeneratedNever()
                .HasColumnName("Cur_ID");
            entity.Property(e => e.CurAbbreviation)
                .HasMaxLength(10)
                .HasColumnName("Cur_Abbreviation");
            entity.Property(e => e.CurName)
                .HasMaxLength(100)
                .HasColumnName("Cur_Name");
            entity.Property(e => e.CurOfficialRate).HasColumnName("Cur_OfficialRate");
            entity.Property(e => e.CurPeriodicity).HasColumnName("Cur_Periodicity");
            entity.Property(e => e.CurScale).HasColumnName("Cur_Scale");
        });

        modelBuilder.Entity<CurrencyAmount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Currency__3214EC271A08E88D");

            entity.ToTable("CurrencyAmount");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CurrencyId).HasColumnName("Currency_ID");

            entity.HasOne(d => d.Currency).WithMany(p => p.CurrencyAmounts)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("FK__CurrencyA__Curre__398D8EEE");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC2796CFCC5B");

            entity.ToTable("Customer");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Created_Date");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("Phone_Number");
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.Customers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Customer__Role_I__3E52440B");
        });

        modelBuilder.Entity<CustomerRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Customer__D80AB49B299A1A75");

            entity.ToTable("CustomerRole");

            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("Role_Name");
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Operatio__3214EC27D637755F");

            entity.ToTable("Operation");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AmountOfDeposited).HasColumnName("Amount_Of_Deposited");
            entity.Property(e => e.AmountOfReceived).HasColumnName("Amount_Of_Received");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Created_Date");
            entity.Property(e => e.CustomerId).HasColumnName("Customer_ID");
            entity.Property(e => e.DepositedСurrencyId).HasColumnName("Deposited_Сurrency_ID");
            entity.Property(e => e.ReceivedCurrencyId).HasColumnName("Received_currency_ID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Operations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operation__Custo__412EB0B6");

            entity.HasOne(d => d.DepositedСurrency).WithMany(p => p.OperationDepositedСurrencies)
                .HasForeignKey(d => d.DepositedСurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operation__Depos__4222D4EF");

            entity.HasOne(d => d.ReceivedCurrency).WithMany(p => p.OperationReceivedCurrencies)
                .HasForeignKey(d => d.ReceivedCurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operation__Recei__4316F928");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
