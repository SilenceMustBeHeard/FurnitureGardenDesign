using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Message
{
    public interface ISystemInboxMessageRepository
     : IRepository<SystemInboxMessage, Guid>,
       IRepositoryAsync<SystemInboxMessage, Guid>
    {
    }
}