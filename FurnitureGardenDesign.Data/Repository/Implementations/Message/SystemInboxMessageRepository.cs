using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Message
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
