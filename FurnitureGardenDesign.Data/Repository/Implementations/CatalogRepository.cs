using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class CatalogRepository
        : BaseRepository<CatalogDesign, Guid>, ICatalogRepository
    {
        public CatalogRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
