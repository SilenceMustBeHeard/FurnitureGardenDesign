using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IDesignVariantService
    {
        Task<IEnumerable<CatalogDesign>> GetAllActiveAsync();
        Task<DesignVariant?> GetByIdAsync(Guid id);
        Task AddDesignVariantAsync(Guid orderId, string imageUrl, string? notes);
        Task ApproveDesignVariantAsync(Guid designVariantId);
        Task DeleteDesignVariantAsync(Guid designVariantId);
    }
}
