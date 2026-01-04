using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class CatalogRepository
        : BaseRepository<CatalogDesign, Guid>
    {
        public CatalogRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
