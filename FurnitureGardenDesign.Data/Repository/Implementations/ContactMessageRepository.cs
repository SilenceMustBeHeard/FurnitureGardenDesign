using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class ContactMessageRepository
    : BaseRepository<ContactMessage, Guid>, IContactMessageRepository
    {
        public ContactMessageRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}