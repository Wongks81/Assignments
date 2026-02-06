using Capstone6.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTest
{
    public static class DbContextMocker
    {
        public static ApplicationDbContext GetApplicationDbContext(string dbname)
        {
            var serviceProvider = new ServiceCollection()
                                    .AddEntityFrameworkInMemoryDatabase()
                                    .BuildServiceProvider();

            // Create the Options for the DbContext instance
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbname)
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            // Create the instance of the DbContext - InMemory database
            var dbContext = new ApplicationDbContext(options);

            dbContext.SeedData();

            return dbContext;
        }

        private static void SeedData(this ApplicationDbContext context)
        {
            context.Departments.AddRange(
                new Capstone6.Models.Department { 
                    DepartmentID=1, 
                    DepartmentName="Language",
                    },
                new Capstone6.Models.Department
                {
                    DepartmentID = 2,
                    DepartmentName = "Science",
                },
                new Capstone6.Models.Department
                {
                    DepartmentID = 3,
                    DepartmentName = "Maths",
                }
            );

            context.SaveChanges();
        }
    }
}
