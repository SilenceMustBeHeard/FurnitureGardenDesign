using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Implementations;
using FurnitureGardenDesign.Data.Repository.Implementations.FurnitureGardenDesign.Data.Repository.Implementations;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels;
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



        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _orderRepo.CountPendingAsync();
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            var orders =  _orderRepo.GetAll();
            return orders
                .Where(o => o.Status ==OrderStatus.Pending) 
                .Select(o => new Order
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