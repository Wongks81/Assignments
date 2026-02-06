using Grocery.WebApp.Data.Enums;
using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grocery.WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyIdentityUser, MyIdentityRole, Guid>
    {
        public DbSet<Product> Products { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Fluent API
            // Add any model related customization, which cannot be done by DataAnnotations

            // Settings for MyIdentityUser
            builder.Entity<MyIdentityUser>().Property(e => e.IsAdminUser).HasDefaultValue(false);
            builder.Entity<MyIdentityUser>().Property(e => e.Gender).HasDefaultValue<MyAppGenderTypes>(MyAppGenderTypes.Male);

            // Settings for Product
            builder.Entity<Product>().Property(e => e.Quantity).HasDefaultValue(0);
            builder.Entity<Product>().Property(e => e.SellingPricePerUnit).HasDefaultValue(0.00);
            //use sql getdate()
            builder.Entity<Product>().Property(e => e.LastUpdateOn).HasDefaultValueSql("getdate()");

            // For addressing CASCADE UPDATE and DELETE 
            // NOTE: (p) Parent Entity (c) Child Entity

            // Every Product will point to 1 record of User
            builder.Entity<Product>()                               // Child Table
                .HasOne<MyIdentityUser>(c => c.CreatedByUser)       // Object of parent in Child
                .WithMany(p => p.ProductCreatedByUser)              // collection of children in parent
                .HasForeignKey(c => c.CreatedByUserId)              // column of child on which FK is estabilsh
                .OnDelete(DeleteBehavior.Restrict);                 // CASCADE DELETE behaviour

            builder.Entity<Product>()                               // Child Table
                .HasOne<MyIdentityUser>(c => c.CreatedByUser)       // Object of parent in Child
                .WithMany(p => p.ProductCreatedByUser)              // collection of children in parent
                .HasForeignKey(c => c.CreatedByUserId)              // column of child on which FK is estabilsh
                .OnDelete(DeleteBehavior.Restrict);                 // CASCADE DELETE behaviour


            base.OnModelCreating(builder);
        }
    }
}
