using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Interactions.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Interactions
{


    public class OrderRepository
        : BaseRepository<Order, Guid>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<int> CountPendingAsync()
        {
            return await _dbSet
                .CountAsync(o => o.Status == OrderStatus.Pending);
        }



        // gets an order with its design variants included
        public async Task<Order?> GetOrderWithVariantsAsync(Guid orderId)
        {
            return await _dbSet
                .Include(o => o.DesignVariants)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }






        // updates the status of an order

        public async Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _dbSet.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _dbSet.Update(order);
                await _context.SaveChangesAsync();
            }
        }

    }


}