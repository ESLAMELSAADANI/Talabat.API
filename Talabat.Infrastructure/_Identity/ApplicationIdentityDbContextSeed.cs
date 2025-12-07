using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Infrastructure._Identity
{
    public static class ApplicationIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    DisplayName = "Eslam Elsaadany",
                    Email = "eslamelsaadany7@outlook.com",
                    UserName = "Eslam.Elsaadany",
                    PhoneNumber = "01022010887"
                };

                await userManager.CreateAsync(user, "P@ssw0rd");
            }
        }
    }
}
