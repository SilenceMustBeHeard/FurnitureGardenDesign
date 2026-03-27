using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureGardenDesign.WebApi.Controllers.Areas.Admin.Interactions
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementControllerApi : ControllerBase
    {
        private readonly IUserService _userService;

        public UserManagementControllerApi(IUserService userService)
        {
            _userService = userService;
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
             
          
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var allUsers = await _userService.GetUserManagmentBoardDataAsync(GetUserId());
            return Ok(allUsers);
        }

       
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] ChangeUserRoleViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.NewRole))
            { 
                return BadRequest(new { error = "Please select a valid role." });
            }

            var result = await _userService.ChangeUserRoleAsync(model, GetUserId());

            if (result.Failed)
                return NotFound(new { error = "User not found!" });

            return Ok(new { message = "User role changed successfully!" });
        }

       
        [HttpPost("disable/{userId}")]
        public async Task<IActionResult> DisableUser(string userId)
        {
            var result = await _userService.DisableUser(userId);

            if (result.Failed)
            {
                return NotFound(new { error = "User not found!" });
            }

            return Ok(new { message = "User disabled successfully!" });
        }
    }
}