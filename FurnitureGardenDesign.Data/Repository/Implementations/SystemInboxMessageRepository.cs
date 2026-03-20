using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class SystemInboxMessageRepository
    : BaseRepository<SystemInboxMessage, Guid>, ISystemInboxMessageRepository
    {
        public SystemInboxMessageRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }

}
