using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Message
{
    public interface IInboxMessageRepository
     : IRepository<InboxMessage, Guid>,
       IRepositoryAsync<InboxMessage, Guid>
    {
    }

}
