using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.Core.Entities.Identity;

namespace Talabat.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]// Post : /api/account/login
        public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new ApiResponse(StatusCodes.Status401Unauthorized,"Invalid Login!"));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(StatusCodes.Status401Unauthorized, "Invalid Login!"));

            return Ok(new UserDTO()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = "This Will Be Token"
            });
        }
    }
}
