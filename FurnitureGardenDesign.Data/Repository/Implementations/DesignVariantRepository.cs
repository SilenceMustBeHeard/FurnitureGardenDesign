using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class DesignVariantRepository:
        BaseRepository<DesignVariant, Guid>, IDesignVariantRepository
    {
        protected DesignVariantRepository(ApplicationDbContext context) 
            : base(context)
        {
        }
    }
}
