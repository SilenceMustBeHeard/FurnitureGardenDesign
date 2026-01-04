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
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context; // DI of DbContext
        }

        // adding order to the db
        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        // gives all categories
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                                 .AsNoTracking()
                                 .Where(c => c.IsActive)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                                 .Include(o => o.User)
                                 .Include(o => o.Category)
                                 .Include(o => o.DesignVariants)
                                 .ToListAsync();
        }


        public async Task<int> CountAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders.CountAsync(predicate);
        }

        // Синхронно: взимане на всички
        public IEnumerable<Order> GetAll()
        {
            return _context.Orders
                           .Include(o => o.User)
                           .Include(o => o.Category)
                           .Include(o => o.DesignVariants)
                           .ToList();
        }


        public int Count()
        {
            return _context.Orders.Count();
        }
    }
}