using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogDesign>> GetAllDesignsAsync();
        Task<CatalogDesign?> GetDesignByIdAsync(Guid id);
    }
}
