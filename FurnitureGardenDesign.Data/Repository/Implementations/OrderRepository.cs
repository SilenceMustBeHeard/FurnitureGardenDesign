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
            _context = context;
        }

        public async Task AddAsync(OrderFormViewModel order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderFormViewModel>> GetAllAsync()
        {
            return await _context.Orders
                                 .Include(o => o.User)
                                 .Include(o => o.Category)
                                 .Include(o => o.DesignVariants)
                                 .ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<OrderFormViewModel, bool>> predicate)
        {
            return await _context.Orders.CountAsync(predicate);
        }

        public IEnumerable<OrderFormViewModel> GetAll()
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