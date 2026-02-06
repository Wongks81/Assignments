using Grocery.WebApp.Data.Enums;
using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grocery.WebApp.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<MyIdentityRole> roleManager)
        {
            foreach( var role in Enum.GetValues(typeof(MyAppRoleTypes)))
            {
                MyIdentityRole roleObj = new MyIdentityRole()
                {
                    Name = role.ToString(),
                    Description = $"The {role} for the Application"
                };
                await roleManager.CreateAsync(roleObj);
            }
        }
    }
}
