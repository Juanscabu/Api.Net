using Application.Enums;
using identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace identity.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default Basic User
            var defaultUser = new ApplicationUser
            {
                UserName = "userBasic",
                Email = "userBasic@gmail.com",
                Nombre = "Martin",
                Apellido = "Guerra",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "1234Pa$word");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());

                }
            }

        }
    }
}
