using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order); // adding an order
        Task<IEnumerable<Category>> GetCategoriesAsync(); // for category dropdown

       
        Task<IEnumerable<Order>> GetAllAsync();

        
        Task<int> CountAsync(Expression<Func<Order, bool>> predicate);

        
        IEnumerable<Order> GetAll();
        int Count();



    }
}
