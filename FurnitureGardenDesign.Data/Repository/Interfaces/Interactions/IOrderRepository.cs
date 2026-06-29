using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Interactions
{
    public interface IOrderRepository
       : IRepository<Order, Guid>, IRepositoryAsync<Order, Guid>
    {
        Task<int> CountPendingAsync();

        Task<Order?> GetOrderWithVariantsAsync(Guid orderId);

        Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus);
    }
}