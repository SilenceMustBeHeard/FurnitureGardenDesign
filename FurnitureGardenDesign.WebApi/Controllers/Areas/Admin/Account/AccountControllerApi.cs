using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.Areas.Admin.Account
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AccountControllerApi : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountControllerApi(UserManager<AppUser> userManager,
         SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

       
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

          
            if (!await _userManager.IsInRoleAsync(user, "User"))
            { 
                await _userManager.AddToRoleAsync(user, "User");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(new { message = "Registration successful!", userId = user.Id });
        }

   
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { error = "Invalid login attempt." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { error = "User not found." });
            }
          
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            return Ok(new
            {
                message = "Login successful!",
                isAdmin = isAdmin,
                userId = user.Id,
                email = user.Email
            });
        }

        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout successful!" });
        }

        
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); 
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            { 
                return NotFound(); 
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            return Ok(new
            {
                userId = user.Id,
                email = user.Email,
                isAdmin = isAdmin
            });
        }
    }
}