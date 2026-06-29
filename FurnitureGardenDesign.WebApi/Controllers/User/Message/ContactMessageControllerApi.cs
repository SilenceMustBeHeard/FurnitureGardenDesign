using FurnitureGardenDesign.Services.Core.Interfaces.Message;
using FurnitureGardenDesign.Web.ViewModels.Messages;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Message;

[Route("api/[controller]")]
[ApiController]
public class ContactMessageControllerApi : ControllerBase
{
    private readonly IContactMessageClientService _contactMessageService;

    public ContactMessageControllerApi
        (IContactMessageClientService contactMessageService)
    {
        _contactMessageService = contactMessageService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ContactMessageCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        await _contactMessageService.SendContactMessageAsync(model, User);

        return Created(string.Empty, new
        {
            message = "Your message has been sent successfully! We'll get back to you soon."
        });
    }
}