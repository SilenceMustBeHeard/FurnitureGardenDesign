using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(OrderFormViewModel order);
        Task<IEnumerable<OrderFormViewModel>> GetAllAsync();
        Task<int> CountAsync(Expression<Func<OrderFormViewModel, bool>> predicate);
        IEnumerable<OrderFormViewModel> GetAll();
        int Count();
    }
}
