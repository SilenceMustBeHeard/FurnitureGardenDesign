using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class DesignVariantRepository:
        BaseRepository<DesignVariant, Guid>, IDesignVariantRepository
    {
        public DesignVariantRepository(ApplicationDbContext context) 
            : base(context)
        {


        }

        public async Task<IEnumerable<DesignVariant>> GetByOrderIdAsync(Guid orderId)
        
            => await _context.DesignVariants
                .Where(dv => dv.OrderId == orderId)
                .ToListAsync();
        
    }
}
