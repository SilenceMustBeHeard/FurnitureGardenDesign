using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IFavoriteService
    {



        Task<bool> ToggleFavoriteAsync(string userId, Guid designId);

    }
}
