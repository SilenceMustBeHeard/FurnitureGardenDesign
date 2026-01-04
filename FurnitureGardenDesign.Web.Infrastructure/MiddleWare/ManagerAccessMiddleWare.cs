using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using FurnitureGardenDesign.Data.Models;

namespace FurnitureGardenDesign.Web.Infrastructure.MiddleWare
{
    public class ManagerAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public ManagerAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<AppUser> userManager)
        {
            var path = context.Request.Path.ToString().ToLower();

            if (path.StartsWith("/manager"))
            {
                var user = context.User;

                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    context.Response.Redirect("/Identity/Account/Login");
                    return;
                }

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var appUser = await userManager.FindByIdAsync(userId);

                if (appUser == null || !await userManager.IsInRoleAsync(appUser, "Manager"))
                {
                    context.Response.Redirect("/Home/AccessDenied");
                    return;
                }
            }

            await _next(context);
        }
    }
}
