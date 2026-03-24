using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Interactions
{
    public interface IDesignVariantRepository
         : IRepository<DesignVariant, Guid>, IRepositoryAsync<DesignVariant, Guid>
    {
        Task<IEnumerable<DesignVariant>> GetByOrderId(Guid orderId);






    }
}

