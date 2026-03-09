using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface IDesignVariantRepository
         : IRepository<DesignVariant, Guid>, IRepositoryAsync<DesignVariant, Guid>
    {
        Task<IEnumerable<DesignVariant>> GetByOrderId(Guid orderId);






    }
}

