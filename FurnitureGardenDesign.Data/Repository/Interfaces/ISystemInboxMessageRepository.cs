using FurnitureGardenDesign.Data.Models;
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
