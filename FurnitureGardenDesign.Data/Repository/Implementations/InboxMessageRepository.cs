using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class InboxMessageRepository
    : BaseRepository<InboxMessage, Guid>, IInboxMessageRepository
    {
        public InboxMessageRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }

}
