using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;

namespace FurnitureGardenDesign.Services.Core.Manager.Interfaces
{
    public interface IManagerDesignVariantService
    {
        Task<IEnumerable<DesignVariant>> GetDesignVariantsByOrderIdAsync(Guid orderId);

        Task<DesignVariant> GetDesignVariantByIdAsync(Guid id);

        Task<DesignVariant> CreateDesignVariantAsync(DesignVariantViewModel model);

        Task UpdateDesignVariantAsync(DesignVariant designVariant);

        Task SendDesignVariantProposalAsync(Guid designVariantId);

        Task DeleteDesignVariantAsync(Guid id);
    }
}