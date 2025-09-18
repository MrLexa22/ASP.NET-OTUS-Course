using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerPreference> CustomerPreferences => Set<CustomerPreference>();
        public DbSet<Preference> Preferences => Set<Preference>();
        public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // BaseEntity.Id как PK
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var idProp = entity.FindProperty("Id");
                if (idProp != null)
                    idProp.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
            }

            // Employee
            modelBuilder.Entity<Employee>(b =>
            {
                b.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
                b.Property(p => p.LastName).HasMaxLength(100).IsRequired();
                b.Property(p => p.Email).HasMaxLength(256).IsRequired();
                b.HasIndex(p => p.Email).IsUnique();

                b.HasMany(p => p.Roles)
                 .WithMany(r => r.Employees);
            });

            // Role
            modelBuilder.Entity<Role>(b =>
            {
                b.Property(p => p.Name).HasMaxLength(100).IsRequired();
                b.Property(p => p.Description).HasMaxLength(256);
            });

            // Customer
            modelBuilder.Entity<Customer>(b =>
            {
                b.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
                b.Property(p => p.LastName).HasMaxLength(100).IsRequired();
                b.Property(p => p.Email).HasMaxLength(256).IsRequired();
                b.HasIndex(p => p.Email).IsUnique();
                b.Property(p => p.PhoneNumber).HasMaxLength(32);
            });

            // Preference
            modelBuilder.Entity<Preference>(b =>
            {
                b.Property(p => p.Name).HasMaxLength(100).IsRequired();
            });

            // PromoCode
            modelBuilder.Entity<PromoCode>(b =>
            {
                b.Property(p => p.Code).HasMaxLength(64).IsRequired();
                b.HasIndex(p => p.Code).IsUnique();
                b.Property(p => p.ServiceInfo).HasMaxLength(256);

                b.HasOne(p => p.Preference)
                 .WithMany(pf => pf.PromoCodes)
                 .HasForeignKey(p => p.PreferenceId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(p => p.Customer)
                 .WithMany(c => c.PromoCodes)
                 .HasForeignKey(p => p.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // CustomerPreference (Many-to-Many через явную таблицу)
            modelBuilder.Entity<CustomerPreference>(b =>
            {
                b.HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

                b.HasOne(cp => cp.Customer)
                 .WithMany(c => c.CustomerPreferences)
                 .HasForeignKey(cp => cp.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(cp => cp.Preference)
                 .WithMany(p => p.CustomerPreferences)
                 .HasForeignKey(cp => cp.PreferenceId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
