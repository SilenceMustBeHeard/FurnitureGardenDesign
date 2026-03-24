using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Interactions
{
    public class DesignVariantRepository :
        BaseRepository<DesignVariant, Guid>, IDesignVariantRepository
    {
        public DesignVariantRepository(ApplicationDbContext context)
            : base(context)
        {


        }

        public async Task<IEnumerable<DesignVariant>> GetByOrderId(Guid orderId)

            => await _context.DesignVariants
                .Where(dv => dv.OrderId == orderId)
                .ToListAsync();



     



    }
}