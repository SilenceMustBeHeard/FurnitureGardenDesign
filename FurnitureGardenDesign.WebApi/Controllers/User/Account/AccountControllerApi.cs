using FurnitureGardenDesign.Services.Core.Interfaces.Account;
using FurnitureGardenDesign.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Account;

[Route("api/[controller]")]
[ApiController]
public class AccountControllerApi : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountControllerApi(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAccount([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _accountService.RegisterAsync(model);

        if (result.Success)
        {
            return CreatedAtAction(nameof(CreateAccount),
                new { email = model.Email },
                new { message = "Account created successfully" });
        }

        return BadRequest(new { Errors = result.Errors });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Error = "Invalid login credentials." });
        }

        var success = await _accountService.LoginAsync(model);

        if (!success)
        {
            return Unauthorized(new { Error = "Invalid username or password." });
        }
        return Ok(new
        {
            message = "Login successful",
            user = new { model.Email }
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return Ok(new { message = "Logout successful" });
    }
}