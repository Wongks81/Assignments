using Capstone6.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone6.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Subject>().Property(e => e.SubjectName).HasMaxLength(50);
        }

        //DB Set for Related Table
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}
