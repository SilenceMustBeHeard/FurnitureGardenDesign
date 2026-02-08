using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Models;

using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels;
using FurnitureGardenDesign.Web.ViewModels.Admin.Order;
using FurnitureGardenDesign.Web.ViewModels.Orders;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }



        // creates new order 

        public async Task CreateOrderAsync(string userId, OrderFormViewModel model)
        {
            var order = new Order
            {
                UserId = userId,
                CategoryId = model.CategoryId,
                FurnitureType = model.FurnitureType,
                Dimensions = model.Dimensions,
                Description = model.Description,
                ReferenceImageUrl = model.ReferenceImageUrl,
                Status = OrderStatus.Pending,
                CreatedOn = DateTime.UtcNow
            };

            await _orderRepo.AddAsync(order);
        }


        // gets the count of all pending orders for admin or manager view
        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _orderRepo.CountPendingAsync();
        }




        // gets all pending orders for admin or manager view
        public async Task<IEnumerable<AdminOrderListViewModel>> GetPendingOrdersAsync()
        {
            return await _orderRepo
                .GetAllAttached()
                .Where(o => o.Status == OrderStatus.Pending)
                .Include(o => o.User)
                .Include(o => o.Category)
                .Select(o => new AdminOrderListViewModel
                {
                    Id = o.Id,
                    UserEmail = o.User.Email,
                    CategoryName = o.Category.Name,
                    Description = o.Description,
                    Status = o.Status,
                    CreatedOn = o.CreatedOn
                })
                .ToListAsync();
        }


        // gets order details by id for admin or manager
        public async Task<DetailsOrderViewModel?> GetByIdAsync(Guid id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null) return null;

            return new DetailsOrderViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                CategoryId = order.CategoryId,
                FurnitureType = order.FurnitureType,
                Dimensions = order.Dimensions,
                Description = order.Description,
                ReferenceImageUrl = order.ReferenceImageUrl,
                Status = order.Status
            };
        }


        // reject order by id for admin or manager(sets status to rejected, imitates soft delete)
        public async Task<bool> RejectOrderAsync(Guid id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
                return false;

            order.Status = OrderStatus.Rejected;

          



            await _orderRepo.SaveChangesAsync();
            return true;
        }









        // deletes the order (still in developement, not used in the application, but can be used for hard delete if needed)
        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
                return false;

        

        
            await _orderRepo.DeleteAsync(order);



            await _orderRepo.SaveChangesAsync();
            return true;
        }






    }
}