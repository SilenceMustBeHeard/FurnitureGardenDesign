using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;

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