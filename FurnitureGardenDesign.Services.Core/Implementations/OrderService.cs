using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Implementations;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task CreateOrderAsync(string userId, OrderFormViewModel model)
        {
            var order = new OrderFormViewModel
            {
                UserId = userId,
                CategoryId = model.CategoryId,
                Description = model.Description,
                Status =OrderStatus.Pending
            };

            await _orderRepo.AddAsync(order);
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _orderRepo.CountAsync(o => o.Status ==OrderStatus.Pending);
        }

        public async Task<IEnumerable<OrderFormViewModel>> GetPendingOrdersAsync()
        {
            var orders = await _orderRepo.GetAllAsync();
            return orders
                .Where(o => o.Status ==OrderStatus.Pending) // LINQ върху IEnumerable
                .Select(o => new OrderFormViewModel
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    CategoryId = o.CategoryId,
                    Description = o.Description,
                    Status = o.Status
                })
                .ToList();
        }
    }
}