using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels;
using FurnitureGardenDesign.Web.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IContactMessageClientService
    {

        Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal);
    }
}

