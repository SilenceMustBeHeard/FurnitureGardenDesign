using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;

namespace FurnitureGardenDesign.Services.Core.Manager.Interfaces
{
    public interface IDesignVariantService
    {
        Task<IEnumerable<DesignVariant>> GetDesignVariantsByOrderIdAsync(Guid orderId);
         Task<DesignVariant> GetDesignVariantByIdAsync(Guid id);

        Task<DesignVariant> CreateDesignVariantAsync(DesignVariantViewModel model);

        Task UpdateDesignVariantAsync(DesignVariant designVariant);

        Task SendDesignVariantProposalAsync(Guid designVariantId);


        Task DeleteDesignVariantAsync(Guid id);


    }

}
