using FurnitureGardenDesign.Data.Models.Messages;
using FurnitureGardenDesign.Data.Repository.Implementations.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Message;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Message
{
    public class ContactMessageRepository
    : BaseRepository<ContactMessage, Guid>, IContactMessageRepository
    {
        public ContactMessageRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}