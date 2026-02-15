using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Data.Repository.Implementations
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

        public async Task<Order?> GetOrderWithVariantsAsync(Guid orderId)
        {
            return await _dbSet
                .Include(o => o.DesignVariants)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }


    }


}