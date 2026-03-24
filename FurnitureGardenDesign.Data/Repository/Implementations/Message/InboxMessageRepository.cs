using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Message
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
