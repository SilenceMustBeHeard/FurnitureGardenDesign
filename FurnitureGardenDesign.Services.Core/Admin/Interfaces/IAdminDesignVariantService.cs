using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{
    public interface IAdminDesignVariantService
    {
        Task<IEnumerable<DesignVariant>> GetDesignVariantsByOrderIdAsync(Guid orderId);
         Task<DesignVariant> GetDesignVariantByIdAsync(Guid id);

        Task<DesignVariant> CreateDesignVariantAsync(DesignVariantViewModel model);

        Task UpdateDesignVariantAsync(DesignVariant designVariant);

        Task SendDesignVariantProposalAsync(Guid designVariantId);


        Task DeleteDesignVariantAsync(Guid id);


    }

}
