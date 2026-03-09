using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;
using FurnitureGardenDesign.Web.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IProfileService
    {

        Task<ProfileViewModel?> GetProfileAsync(string userId);
      






    }
}
