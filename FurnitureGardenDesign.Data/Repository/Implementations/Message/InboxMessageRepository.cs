using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;

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