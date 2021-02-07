using leave_mangment.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment
{
    public static class SeedData
    {
        public static void Seed(UserManager<Employee> userManager,
            RoleManager<IdentityRole> roleManager) 
        {
            SeedRole(roleManager);
            SeedUser(userManager);
        }

        private static void SeedRole(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
                var result = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Employee").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Employee"
                };
                var result = roleManager.CreateAsync(role).Result;
            }
        }



        private static void SeedUser(UserManager<Employee> userManager) 
        {
            if (userManager.FindByNameAsync("Admin").Result == null) 
            {
                var user = new Employee
                {
                    UserName = "admin@localhost.com", //Full email address
                    Email = "admin@localhost.com" //Full email address
                };

                var result = userManager.CreateAsync(user, "123456qQ@").Result;
                if (result.Succeeded) 
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }

    }
}
