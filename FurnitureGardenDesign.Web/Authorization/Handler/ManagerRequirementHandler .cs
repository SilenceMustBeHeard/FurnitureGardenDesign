using FurnitureGardenDesign.Services.Core.Interfaces.Account;
using FurnitureGardenDesign.Web.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FurnitureGardenDesign.Web.Authorization.Handlers
{
    public class ManagerRequirementHandler : AuthorizationHandler<ManagerRequirement>
    {
        private readonly IManagerService _managerService;

        public ManagerRequirementHandler(IManagerService managerService)
        {
            _managerService = managerService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManagerRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                context.Fail();
                return;
            }

            bool isManager = await _managerService.IsUserManagerAsync(userId);
            if (isManager)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}