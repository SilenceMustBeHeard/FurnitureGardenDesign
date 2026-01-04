using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class CategoryRepository
        : BaseRepository<Category, Guid>
    {
        public CategoryRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }

}
