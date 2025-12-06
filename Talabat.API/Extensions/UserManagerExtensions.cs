using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<ApplicationUser?> FindUserWithAddressByEmailAsync(this UserManager<ApplicationUser> userManager,ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper());

            return user;
        }
    }
}
