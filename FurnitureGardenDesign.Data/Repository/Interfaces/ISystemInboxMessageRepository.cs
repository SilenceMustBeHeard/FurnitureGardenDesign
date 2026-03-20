using FurnitureGardenDesign.Data.Models.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface ISystemInboxMessageRepository
     : IRepository<SystemInboxMessage, Guid>,
       IRepositoryAsync<SystemInboxMessage, Guid>
    {
    }

}
