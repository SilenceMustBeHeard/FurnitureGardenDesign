using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FurnitureGardenDesign.WebApi.Controllers.User.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeControllerApi : ControllerBase
    {
       
        [HttpPost("index")]
        public IActionResult Index() => Ok();

        [HttpPost("about")]
        public IActionResult About() => Ok();

      
        [HttpPost("privacy")]
        public IActionResult Privacy() => Ok();

     
    }
}


