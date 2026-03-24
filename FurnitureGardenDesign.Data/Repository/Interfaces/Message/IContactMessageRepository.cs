using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Message
{
    public interface IContactMessageRepository
        : IRepository<ContactMessage, Guid>,
       IRepositoryAsync<ContactMessage, Guid>
    {
    }

}
